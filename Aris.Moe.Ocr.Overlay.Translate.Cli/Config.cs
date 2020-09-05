using System.Drawing;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class Config : IGoogleConfiguration, IOcrTranslateOverlayConfiguration
    {
        public string? KeyPath { get; set; } = @"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private\key.json";
        public Rectangle ScreenArea { get; set; } = new Rectangle(new Point(310, 126), new Size(1281, 904));
        public string? SourceLanguage { get; set; } = "ja";
        public string TargetLanguage { get; set; } = "en";
        public bool PermanentlyCacheExternalOcrResult { get; set; } = false;
        public bool PermanentlyCacheExternalTranslateResult { get; set; } = false;
        public string CacheFolderRoot { get; set; } = @"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private";
    }
}