using System;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay
{
    public interface IGuiMode
    {
        bool ShouldRender { get; }
        event EventHandler? OnWantsToRender;
        void Render();
    }
}