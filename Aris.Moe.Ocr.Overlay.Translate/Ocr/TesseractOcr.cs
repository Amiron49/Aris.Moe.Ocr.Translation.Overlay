using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Tesseract;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class StreamHelper
    {
        public static async Task<MemoryStream> CreateStreamCopy(Stream image)
        {
            var memoryStream = new MemoryStream();

            image.Position = 0;
            await image.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }


    public class StreamProgressTracker : Stream
    {
        private readonly Stream _streamImplementation;
        private readonly IProgress<double> _progress;

        public StreamProgressTracker(Stream streamImplementation, IProgress<double> progress)
        {
            _streamImplementation = streamImplementation;
            _progress = progress;
        }

        public override void Flush()
        {
            _streamImplementation.Flush();
            _progress.Report(1);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _streamImplementation.Read(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var readAsync = await base.ReadAsync(buffer, offset, count, cancellationToken);

            var done = Position + readAsync;
            var progress = (double) done / Length;

            _progress.Report(progress);

            return readAsync;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _streamImplementation.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _streamImplementation.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _streamImplementation.Write(buffer, offset, count);
        }

        public override bool CanRead => _streamImplementation.CanRead;

        public override bool CanSeek => _streamImplementation.CanSeek;

        public override bool CanWrite => _streamImplementation.CanWrite;

        public override long Length => _streamImplementation.Length;

        public override long Position
        {
            get => _streamImplementation.Position;
            set => _streamImplementation.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            _streamImplementation.Dispose();
        }
    }

    public class TesseractOcr : IOcr
    {
        private readonly ILogger<TesseractOcr> _logger;
        private readonly RegionsOfInterestFinder _regionsOfInterestFinder;

        public TesseractOcr(ILogger<TesseractOcr> logger, IProgressOverlay progressOverlay)
        {
            _logger = logger;
            _regionsOfInterestFinder = new RegionsOfInterestFinder(logger, progressOverlay);
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            using var engine = new TesseractEngine(@"./tessdata", MapLanguage(inputLanguage!), EngineMode.Default);
            engine.SetVariable("tessedit_write_images", true);

            var regions = await _regionsOfInterestFinder.GetInterestRegions(image);
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

        private async Task<IEnumerable<(Stream cutout, Rectangle area)>> SplitIntoOwnImages(Stream image, IEnumerable<Rectangle> areas)
        {
            var results = new List<(Stream cutout, Rectangle area)>();

            foreach (var area in areas)
            {
                using var streamCopy = await StreamHelper.CreateStreamCopy(image);

                using var editImage = await SixLabors.ImageSharp.Image.LoadAsync(streamCopy);
                editImage.Mutate(x => x.Crop(new SixLabors.ImageSharp.Rectangle(area.X, area.Y, area.Width, area.Height)));

                var saveStream = new MemoryStream();

                await editImage.SaveAsync(saveStream, new PngEncoder());
                saveStream.Position = 0;

                results.Add((saveStream, area));
            }

            return results;
        }

        private async Task<IEnumerable<ISpatialText>> OcrInternal(Stream image, TesseractEngine engine)
        {
            var memoryStream = await StreamHelper.CreateStreamCopy(image);

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
    }

    public class OverlapComparer : IEqualityComparer<Rectangle>
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