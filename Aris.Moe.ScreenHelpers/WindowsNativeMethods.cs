using System.Drawing;
using System.Runtime.InteropServices;

namespace Aris.Moe.ScreenHelpers
{
    /// <summary>
    /// :^(
    /// </summary>
    public static class WindowsNativeMethods
    {
        public static Rectangle VirtualScreen
        {
            get
            {
                if (MultiMonitorSupport)
                    return new Rectangle(GetSystemMetrics(76), GetSystemMetrics(77), GetSystemMetrics(78), GetSystemMetrics(79));
                var primaryMonitorSize = PrimaryMonitorSize;
                return new Rectangle(0, 0, primaryMonitorSize.Width, primaryMonitorSize.Height);
            }
        }

        private static bool MultiMonitorSupport  => GetSystemMetrics(80) > 0U;

        private static Size PrimaryMonitorSize => new Size(GetSystemMetrics(0), GetSystemMetrics(1));

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetSystemMetrics(int nIndex);
    }
}