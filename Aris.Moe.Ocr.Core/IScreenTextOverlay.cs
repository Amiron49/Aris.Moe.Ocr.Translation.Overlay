namespace Aris.Moe.Ocr.Core
{
    public interface IScreenTextOverlay
    {
        void Add(SpatialText text);
        void ClearAll();
    }
}