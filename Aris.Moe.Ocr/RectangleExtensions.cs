using System.Drawing;

namespace Aris.Moe.Ocr
{
    public static class RectangleExtensions
    {
        public static bool IsInDistance(this Rectangle a, Rectangle b, double distance)
        {
            var halfDistance = distance / 2;

            var aCopy = Rectangle.Inflate(a, (int) halfDistance, (int) halfDistance);
            var bCopy = Rectangle.Inflate(b, (int) halfDistance, (int) halfDistance);

            return aCopy.IntersectsWith(bCopy);
        }
        
        public static double RelativeSizeTo(this Rectangle a, Rectangle b)
        {
            var aArea = a.Height * a.Width;
            var bArea = b.Height * b.Width;

            return (double)aArea / bArea;
        }
    }
}