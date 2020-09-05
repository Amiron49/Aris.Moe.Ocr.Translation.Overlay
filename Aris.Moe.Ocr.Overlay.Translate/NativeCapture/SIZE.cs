using System.Drawing;
using System.Runtime.InteropServices;

namespace Aris.Moe.Ocr.Overlay.Translate.NativeCapture
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int Width;
        public int Height;

        public SIZE(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static explicit operator Size(SIZE s)
        {
            return new Size(s.Width, s.Height);
        }

        public static explicit operator SIZE(Size s)
        {
            return new SIZE(s.Width, s.Height);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }
    }
}