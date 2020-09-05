using System.Drawing;

namespace Aris.Moe.Ocr.Overlay.Translate
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
    }
}