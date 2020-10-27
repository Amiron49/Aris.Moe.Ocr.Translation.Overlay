using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    public class ControlsModel : IOcrTranslateOverlay
    {
        private readonly IOcrTranslateOverlay _translateOverlay;
        
        public ControlsModel()
        {
            _translateOverlay = Program.Services.GetInstance<IOcrTranslateOverlay>();
        }

        public async Task TranslateScreen()
        {
            await _translateOverlay.TranslateScreen();
        }

        public void HideOverlay()
        {
            _translateOverlay.HideOverlay();
        }

        public void ToggleOverlay()
        {
            _translateOverlay.ToggleOverlay();
        }

        public void ShowOverlay()
        {
            _translateOverlay.ShowOverlay();
        }

        public async Task OcrScreen()
        {
            await _translateOverlay.OcrScreen();
        }

        public void AskForTargetResize()
        {
            _translateOverlay.AskForTargetResize();
        }

        public void Dispose()
        {
        }
    }
}