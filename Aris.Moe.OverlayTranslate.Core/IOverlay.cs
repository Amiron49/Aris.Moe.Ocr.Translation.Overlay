using System;
using System.Threading.Tasks;

namespace Aris.Moe.Core
{
}

namespace Aris.Moe.OverlayTranslate.Core
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