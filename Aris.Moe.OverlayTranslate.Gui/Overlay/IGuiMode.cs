using System;

namespace Aris.Moe.OverlayTranslate.Gui.Overlay
{
    public interface IGuiMode
    {
        bool ShouldRender { get; }
        event EventHandler? OnWantsToRender;
        void Render();
    }
}