using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.Configuration;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.Ocr.Overlay.Translate.DependencyInjection;
using Aris.Moe.Translate;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public class OverlayTranslateGuiRegistry : ServiceRegistry
    {
        public OverlayTranslateGuiRegistry(Configuration configuration)
        {
            var logger = CreateLogger(configuration.Logging);
            
            IncludeRegistry<OverlayTranslateRegistry>();
            For<IOcr>().Use<OcrMediator>();
            For<ITranslate>().Use<TranslationMediator>();
            
            this.AddLogging(builder => builder.AddSerilog(logger));

            For<IOcrTranslateOverlayConfiguration>().Use(configuration).Singleton();
            For<IGoogleConfiguration>().Use(configuration.Google).Singleton();
            For<IDeeplConfiguration>().Use(configuration.Deepl).Singleton();
        }
        
        private static ILogger CreateLogger(ILoggingConfiguration configuration) {
            var builder =  new LoggerConfiguration();

            if (configuration.Verbose)
                builder = builder.MinimumLevel.Debug();
            else
                builder = builder.MinimumLevel.Information();

            if (configuration.FileLogging)
            {
                var fileLoggingLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "logs", "regular.log");
                builder = builder.WriteTo.File(fileLoggingLocation);
            }

            if (configuration.DebugLogging)
                builder = builder.WriteTo.Debug();

            return builder.CreateLogger();
        }
        
    }
    
    public class OcrMediator: IOcr
    {
        private readonly IOcrTranslateOverlayConfiguration _ocrTranslateOverlayConfiguration;
        private readonly Lazy<GoogleOcr> _googleOcr;
        private readonly Lazy<TesseractOcr> _tesseractOcr;

        public OcrMediator(IOcrTranslateOverlayConfiguration ocrTranslateOverlayConfiguration, Lazy<GoogleOcr> googleOcr, Lazy<TesseractOcr> tesseractOcr)
        {
            _ocrTranslateOverlayConfiguration = ocrTranslateOverlayConfiguration;
            _googleOcr = googleOcr;
            _tesseractOcr = tesseractOcr;
        }
        
        public Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            return _ocrTranslateOverlayConfiguration.OcrProvider switch
            {
                "Google" => _googleOcr.Value.Ocr(image, inputLanguage),
                "Tesseract" => _tesseractOcr.Value.Ocr(image, inputLanguage),
                _ => throw new Exception($"{_ocrTranslateOverlayConfiguration.OcrProvider} is not a known provider")
            };
        }
    }
    
    public class TranslationMediator: ITranslate
    {
        private readonly IOcrTranslateOverlayConfiguration _ocrTranslateOverlayConfiguration;
        private readonly Lazy<GoogleTranslate> _googleTranslate;
        private readonly Lazy<DeeplTranslate> _deeplTranslate;

        public TranslationMediator(IOcrTranslateOverlayConfiguration ocrTranslateOverlayConfiguration, Lazy<GoogleTranslate> googleTranslate, Lazy<DeeplTranslate> deeplTranslate)
        {
            _ocrTranslateOverlayConfiguration = ocrTranslateOverlayConfiguration;
            _googleTranslate = googleTranslate;
            _deeplTranslate = deeplTranslate;
        }
        

        public Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            return _ocrTranslateOverlayConfiguration.TranslationProvider switch
            {
                "Google" => _googleTranslate.Value.Translate(spatialTexts, targetLanguage, inputLanguage),
                "Deepl" => _deeplTranslate.Value.Translate(spatialTexts, targetLanguage, inputLanguage),
                _ => throw new Exception($"{_ocrTranslateOverlayConfiguration.TranslationProvider} is not a known provider")
            };
        }
    }
}