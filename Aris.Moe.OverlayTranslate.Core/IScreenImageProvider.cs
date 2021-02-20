using System.Drawing;
using System.IO;

namespace Aris.Moe.OverlayTranslate.Core
{
    public interface IScreenImageProvider
    {
        (Stream Stream, Bitmap Original) Get(Rectangle area);
    }
}