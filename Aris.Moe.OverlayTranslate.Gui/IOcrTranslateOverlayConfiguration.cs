using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public interface IOcrTranslateOverlayConfiguration
    {
        Rectangle CaptureArea { get; set; }
        string? SourceLanguage { get; set; }
        string TargetLanguage { get; set; }

        /// <summary>
        /// Permanently caches any OCR request, for debugging purposes.
        /// </summary>
        bool PermanentlyCacheExternalOcrResult { get; set; }

        bool PermanentlyCacheExternalTranslateResult { get; set; }
        bool DebugCapturedImage { get; set; }
        string CacheFolderRoot { get; set; }
        string OcrProvider { get; set; }
        string TranslationProvider { get; set; }
    }
}