using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Gui.Overlay.Modes
{
    internal static class PointExtensions
    {
        public static Rectangle? ToRectangleWithUnknownPointOrder(this Point a, Point b)
        {
            var pointsAreOnTheSameLine = a.X == b.X || a.Y == b.Y;

            if (pointsAreOnTheSameLine)
                return null;

            var aHasLowestX = a.X < b.X;
            var aHasLowestY = a.Y < b.Y;

            var lowestX = aHasLowestX ? a.X : b.X;
            var lowestY = aHasLowestY ? a.Y : b.Y;

            var highestX = aHasLowestX ? b.X : a.X;
            var highestY = aHasLowestY ? b.Y : a.Y;

            var topLeft = new Point(lowestX, lowestY);
            var bottomRight = new Point(highestX, highestY);

            return topLeft.ToRectangle(bottomRight);
        }

        private static Rectangle ToRectangle(this Point topLeft, Point bottomRight)
        {
            return new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
        }
    }
}