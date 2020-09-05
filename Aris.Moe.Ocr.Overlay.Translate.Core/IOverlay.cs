using System;
using System.Drawing;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IOverlay : IDisposable
    {
        void Add(params ISpatialText[] texts);
        void Add(Bitmap image, Rectangle targetArea);
        void ClearAll();

        public void HideOverlay();
        public void ShowOverlay();
        public void ToggleOverlay();
    }
}