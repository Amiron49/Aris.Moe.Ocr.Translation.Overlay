using System.Drawing;
using System.IO;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IScreenImageProvider
    {
        (Stream Stream, Bitmap Original) Get(Rectangle area);
    }
}