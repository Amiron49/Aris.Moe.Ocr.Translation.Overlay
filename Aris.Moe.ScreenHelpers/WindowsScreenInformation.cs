using System.Drawing;

namespace Aris.Moe.ScreenHelpers
{
    public class WindowsScreenInformation : IScreenInformation
    {
        public Rectangle ScreenArea => WindowsNativeMethods.VirtualScreen;
    }
}