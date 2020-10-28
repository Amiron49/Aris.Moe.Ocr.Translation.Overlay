using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IOverlay : ITextOverlay, ITargetAreaResizeOverlay, IDisposable
    {
        public bool Ready { get; }
        public void HideOverlay();
        public void ShowOverlay();
        public void ToggleOverlay();
        public Task Init();
    }

    public interface ITextOverlay
    {
        void Add(params ISpatialText[] texts);
        void ClearAll();
    }

    public interface ITargetAreaResizeOverlay
    {
        void AskForResize(Rectangle current, Action<Rectangle?> resultCallback);
    }
}