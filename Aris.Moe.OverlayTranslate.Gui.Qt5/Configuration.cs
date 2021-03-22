using System;
using System.Drawing;
using System.IO;
using Aris.Moe.OverlayTranslate.Configuration;

namespace Aris.Moe.OverlayTranslate.Gui.Qt5
{
    public class Configuration : BaseConfiguration, IOcrTranslateOverlayConfiguration
    {
        public Rectangle CaptureArea { get; set; } = new Rectangle(new Point(310, 126), new Size(1281, 904));
        public string? SourceLanguage { get; set; } = "ja";
        public string TargetLanguage { get; set; } = "en";
        public bool PermanentlyCacheExternalOcrResult { get; set; } = false;
        public bool PermanentlyCacheExternalTranslateResult { get; set; } = false;
        public bool DebugCapturedImage { get; set; } = false;
        public string CacheFolderRoot { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, ".cache");

        public string OcrProvider { get; set; } = "Tesseract";
        public string TranslationProvider { get; set; } = "Deepl";

        public LoggingConfiguration Logging { get; set; }
    }
}