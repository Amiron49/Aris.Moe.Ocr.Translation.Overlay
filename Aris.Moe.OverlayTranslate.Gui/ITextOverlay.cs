using Aris.Moe.Ocr;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public interface ITextOverlay
    {
        void Add(params ISpatialText[] texts);
        void ClearAll();
    }
}