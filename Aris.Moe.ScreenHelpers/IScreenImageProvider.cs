using System.Drawing;
using System.IO;

namespace Aris.Moe.ScreenHelpers
{
    public interface IScreenImageProvider
    {
        (Stream Stream, Bitmap Original) Get(Rectangle area);
    }
}