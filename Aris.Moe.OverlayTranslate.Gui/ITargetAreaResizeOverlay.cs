using System;
using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public interface ITargetAreaResizeOverlay
    {
        void AskForResize(Rectangle current, Action<Rectangle?> resultCallback);
    }
}