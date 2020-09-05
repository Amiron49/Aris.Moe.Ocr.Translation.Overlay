using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Lamar;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var config = new Config();

            var container = new Container(new OverlayTranslateCliRegistry(config));

            var ocrTranslateOverlay = container.GetInstance<IOcrTranslateOverlay>();

            ocrTranslateOverlay.ShowOverlay();

            var result = "";
            do
            {
                Console.WriteLine("t: translate");
                Console.WriteLine("o: toggle overlay");
                Console.WriteLine("r: ocr the screen");
                Console.WriteLine("x: exit");
                Console.WriteLine("Press any of the above keys: ");
                result = Console.ReadKey().KeyChar.ToString();

                Console.WriteLine();

                if (result == "t")
                {
                    await ocrTranslateOverlay.TranslateScreen();
                }
                else if (result == "o")
                {
                    ocrTranslateOverlay.ToggleOverlay();
                }
                else if (result == "r")
                {
                    await ocrTranslateOverlay.OcrScreen();
                }
            } while (result != "x");
        }
    }

    public class OverlayTranslateCliRegistry : ServiceRegistry
    {
        public OverlayTranslateCliRegistry(Config configuration)
        {
            For<Action<string>>().Use(Console.WriteLine);
            For<IOcrTranslateOverlay>().Use<OcrTranslateOverlay>();
            For<IScreenImageProvider>().Use<ScreenProvider>();
            For<IOverlay>().Use<Overlay>();
            For<IOcr>().Use<GoogleOcr>();
            For<IOcr>().DecorateAllWith<OcrDebugCache>();
            For<IOcr>().DecorateAllWith<OcrCoordinator>();
            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
            For<ITranslate>().Use<GoogleTranslate>();
            For<ITranslate>().DecorateAllWith<TranslateDebugCache>();
            For<IOcrTranslateOverlayConfiguration>().Use(configuration);
            For<IGoogleConfiguration>().Use(configuration);
        }
    }
    
    public class OcrDebugCache: IOcr
    {
        private readonly IOcr _decorated;
        private readonly IOcrTranslateOverlayConfiguration _configuration;

        public OcrDebugCache(IOcr decorated, IOcrTranslateOverlayConfiguration configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }
        
        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            if (!_configuration.PermanentlyCacheExternalOcrResult)
                return await _decorated.Ocr(image, inputLanguage);

            var result = GetCached();

            if (result == null)
            {
                result = (await _decorated.Ocr(image, inputLanguage)).ToList();
                Cache(result);
            }

            return result;
        }

        private List<ISpatialText>? GetCached()
        {
            var cacheFilePath = _configuration.CacheFolderRoot + @"\ocr_cache.json";
            
            var cacheExists = File.Exists(cacheFilePath);

            if (!cacheExists)
                return null;

            var content = File.ReadAllText(cacheFilePath);

            if (string.IsNullOrEmpty(content))
                return null;
            
            var deserialized = JsonConvert.DeserializeObject<List<SpatialText>>(content);

            return new List<ISpatialText>(deserialized);
        }
        
        private void Cache(IEnumerable<ISpatialText> texts)
        {
            var cacheFilePath = _configuration.CacheFolderRoot + @"\ocr_cache.json";
            
            var serialised = JsonConvert.SerializeObject(texts, Formatting.Indented);
            
            File.WriteAllText(cacheFilePath, serialised);
        }
    }
    
    
    public class TranslateDebugCache: ITranslate
    {
        private readonly ITranslate _decorated;
        private readonly IOcrTranslateOverlayConfiguration _configuration;

        public TranslateDebugCache(ITranslate decorated, IOcrTranslateOverlayConfiguration configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            if (!_configuration.PermanentlyCacheExternalOcrResult)
                return await _decorated.Translate(spatialTexts, targetLanguage, inputLanguage);

            var result = GetCached();

            if (result == null)
            {
                result = (await _decorated.Translate(spatialTexts, targetLanguage, inputLanguage)).ToList();
                Cache(result);
            }

            return result;
        }
        
        private List<Translation>? GetCached()
        {
            var cacheFilePath = _configuration.CacheFolderRoot + @"\translate_cache.json";
            
            var cacheExists = File.Exists(cacheFilePath);

            if (!cacheExists)
                return null;

            var content = File.ReadAllText(cacheFilePath);

            if (string.IsNullOrEmpty(content))
                return null;
            
            var deserialized = JsonConvert.DeserializeObject<List<Translation>>(content);

            return new List<Translation>(deserialized);
        }

        private void Cache(IEnumerable<Translation> texts)
        {
            var cacheFilePath = _configuration.CacheFolderRoot + @"\translate_cache.json";
            
            var serialised = JsonConvert.SerializeObject(texts, Formatting.Indented);
            
            File.WriteAllText(cacheFilePath, serialised);
        }
    }

    public class GoddamnitIHateStructsRectangle
    {
        public GoddamnitIHateStructsPoint? Point { get; set; }
        public GoddamnitIHateStructsSize? Size { get; set; }

        public Rectangle CreateRectangle()
        {
            if (Point == null)
                throw new ArgumentNullException(nameof(Point));

            if (Size == null)
                throw new ArgumentNullException(nameof(Size));

            return new Rectangle(new Point(Point.X, Point.Y), new Size(Size.Width, Size.Height));
        }
    }

    public class GoddamnitIHateStructsPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class GoddamnitIHateStructsSize
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}