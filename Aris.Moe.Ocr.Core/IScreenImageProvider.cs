using System.Drawing;

namespace Aris.Moe.Ocr.Core
{
    public interface IScreenImageProvider
    {
        object Get(Rectangle area);
    }
}