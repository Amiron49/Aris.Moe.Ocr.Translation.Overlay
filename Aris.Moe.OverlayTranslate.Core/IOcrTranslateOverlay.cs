using System;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Core
{
    public interface IOcrTranslateOverlay : IDisposable
    {
        Task TranslateScreen();
        void HideOverlay();
        void ToggleOverlay();
        void ShowOverlay();
        Task OcrScreen();
        void AskForTargetResize();
        void DisplayProgress();
    }
}