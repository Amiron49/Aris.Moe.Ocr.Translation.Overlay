using System;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public interface IOverlay : ITextOverlay, ITargetAreaResizeOverlay , IDisposable
    {
        public bool Ready { get; }
        public void HideOverlay();
        public void ShowOverlay();
        public void ToggleOverlay();
        public Task Init();
    }
}