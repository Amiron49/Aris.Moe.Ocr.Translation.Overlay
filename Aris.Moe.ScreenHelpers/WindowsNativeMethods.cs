using System.Drawing;
using System.Runtime.InteropServices;

namespace Aris.Moe.ScreenHelpers
{
    /// <summary>
    /// :^(
    /// </summary>
    public static class WindowsNativeMethods
    {
        private static bool _didTheDpiThing;
        
        public static Rectangle VirtualScreen
        {
            get
            {
                if (!_didTheDpiThing)
                {
                    SetProcessDPIAware();
                    _didTheDpiThing = true;
                }

                if (MultiMonitorSupport)
                {
                    var rectangle = new Rectangle(GetSystemMetrics(76), GetSystemMetrics(77), GetSystemMetrics(78), GetSystemMetrics(79));
                    return rectangle;
                }

                var primaryMonitorSize = PrimaryMonitorSize;

                var virtualScreen = new Rectangle(0, 0, primaryMonitorSize.Width, primaryMonitorSize.Height);
                return virtualScreen;
            }
            
        }

        private static bool MultiMonitorSupport  => GetSystemMetrics(80) > 0U;

        private static Size PrimaryMonitorSize => new Size(GetSystemMetrics(0), GetSystemMetrics(1));

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}