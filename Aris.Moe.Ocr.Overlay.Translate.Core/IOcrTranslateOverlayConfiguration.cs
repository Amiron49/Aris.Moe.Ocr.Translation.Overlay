using System.Drawing;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IOcrTranslateOverlayConfiguration
    {
        Rectangle ScreenArea { get; set; }
        string? SourceLanguage { get; set; }
        string TargetLanguage { get; set; }
        /// <summary>
        /// Permanently caches any OCR request, for debugging purposes.
        /// </summary>
        bool PermanentlyCacheExternalOcrResult { get; set; }
        bool PermanentlyCacheExternalTranslateResult { get; set; }
        string CacheFolderRoot { get; set; }
    }
}