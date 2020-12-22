using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alturos.Yolo;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class TesseractOcr : IOcr
    {
        private readonly ILogger<TesseractOcr> _logger;

        public TesseractOcr(ILogger<TesseractOcr> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            using var engine = new TesseractEngine(@"./tessdata", MapLanguage(inputLanguage!), EngineMode.Default);
            engine.SetVariable("tessedit_write_images", true);

            var regions = await GetInterestRegions(image);
            var splitRegions = await SplitIntoOwnImages(image, regions);
            
            var results = new List<ISpatialText>();

            foreach (var (cutout, area) in splitRegions)
            {
                var ocrResult = (await OcrInternal(cutout, engine)).ToList();

                foreach (var spatialText in ocrResult)
                    spatialText.Move(area.Location);
                
                results.AddRange(ocrResult);
            }

            return results;
        }
        
        private static string MapLanguage(string language)
        {
            return language switch
            {
                "ja" => "jpn",
                _ => language
            };
        }
        
        public enum RegionType
        {
            Text,
            Box,
            Bubble
        }

        private async Task<IEnumerable<(Stream cutout, Rectangle area)>> SplitIntoOwnImages(Stream image, IEnumerable<Rectangle> areas)
        {
            var results = new List<(Stream cutout, Rectangle area)>(); 
            
            foreach (var area in areas)
            {
                using var streamCopy = await CreateStreamCopy(image);

                using var editImage = await SixLabors.ImageSharp.Image.LoadAsync(streamCopy);
                editImage.Mutate(x => x.Crop(new SixLabors.ImageSharp.Rectangle(area.X, area.Y, area.Width, area.Height)));
                
                var saveStream = new MemoryStream();

                await editImage.SaveAsync(saveStream, new PngEncoder());
                saveStream.Position = 0;
                
                results.Add((saveStream, area));
            }

            return results;
        }
        
        private async Task<IEnumerable<Rectangle>> GetInterestRegions(Stream image)
        {
            var allRegions = await GetAllRegions(image);

            var groupedByOverlap = allRegions.GroupBy(x => x.Area, new OverlapComparer());

            var consolidated = groupedByOverlap.Select(group =>
            {
                var texts = group.Where(x => x.Type == RegionType.Text).ToList();

                if (texts.Any())
                    return texts.Select(x => x.Area).Aggregate(Rectangle.Union);

                return group.Select(x => x.Area).Aggregate(Rectangle.Union);
            });

            return consolidated;
        }
        
        private async Task<IEnumerable<(Rectangle Area, RegionType Type)>> GetAllRegions(Stream image)
        {
            var memoryStream = await CreateStreamCopy(image);
            
            using var yoloWrapper = new YoloWrapper(@".\Yolo\yolo-manga.cfg",
                @".\Yolo\yolo-manga.weights", @".\Yolo\yolo-manga.names");

            var items = yoloWrapper.Detect(memoryStream.ToArray());

            return items.Select(x => (new Rectangle(x.X, x.Y, x.Width, x.Height), Convert(x.Type)));
        }

        private RegionType Convert(string type)
        {
            return type switch
            {
                "free-text" => RegionType.Text,
                "box" => RegionType.Box,
                "bubble" => RegionType.Bubble,
                _ => throw new ArgumentOutOfRangeException($"{type} is not known")
            };
        }

        public async Task<IEnumerable<ISpatialText>> OcrInternal(Stream image, TesseractEngine engine)
        {
            var memoryStream = await CreateStreamCopy(image);

            using var img = Pix.LoadFromMemory(memoryStream.ToArray());
            using var page = engine.Process(img, PageSegMode.Auto);

            var completeText = page.GetText();
            _logger.LogInformation("Mean confidence: {0}", page.GetMeanConfidence());

            var results = new List<ISpatialText>();

            using var iter = page.GetIterator();
            iter.Begin();

            do
            {
                do
                {
                    do
                    {
                        do
                        {
                            var boundingWorked = iter.TryGetBoundingBox(PageIteratorLevel.Word, out var boundingBox);
                            var text = iter.GetText(PageIteratorLevel.Word);

                            if (boundingWorked && text != null)
                                results.Add(new SpatialText(text, new Rectangle(boundingBox.X1, boundingBox.Y1, boundingBox.Width, boundingBox.Height)));
                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
            } while (iter.Next(PageIteratorLevel.Block));

            return results;
        }

        private static async Task<MemoryStream> CreateStreamCopy(Stream image)
        {
            var memoryStream = new MemoryStream();

            image.Position = 0;
            await image.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
    
    public class OverlapComparer: IEqualityComparer<Rectangle>
    {
        public bool Equals(Rectangle x, Rectangle y)
        {
            var intersectsWith = x.IntersectsWith(y);

            if (!intersectsWith)
                return false;

            var combined = Rectangle.Intersect(x, y);

            var relativeSizeToX = x.RelativeSizeTo(combined);
            var relativeSizeToY = y.RelativeSizeTo(combined);
            
            return (relativeSizeToX > 0.6 && relativeSizeToY > 1.6) || (relativeSizeToX > 1.6 && relativeSizeToY > 0.6);
        }

        public int GetHashCode(Rectangle obj)
        {
            return 0;
        }
    }
}