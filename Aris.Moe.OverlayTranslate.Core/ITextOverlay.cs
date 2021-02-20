using Aris.Moe.Ocr;

namespace Aris.Moe.OverlayTranslate.Core
{
    public interface ITextOverlay
    {
        void Add(params ISpatialText[] texts);
        void ClearAll();
    }
}