using System;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IOcrTranslateOverlay : IDisposable
    {
        Task TranslateScreen();
        void HideOverlay();
        void ToggleOverlay();
        void ShowOverlay();
        Task OcrScreen();
        void AskForTargetResize();
    }
}