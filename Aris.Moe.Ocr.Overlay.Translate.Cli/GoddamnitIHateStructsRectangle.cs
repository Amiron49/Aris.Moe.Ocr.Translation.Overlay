using System;
using System.Drawing;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class GoddamnitIHateStructsRectangle
    {
        public GoddamnitIHateStructsPoint? Point { get; set; }
        public GoddamnitIHateStructsSize? Size { get; set; }

        public Rectangle CreateRectangle()
        {
            if (Point == null)
                throw new ArgumentNullException(nameof(Point));

            if (Size == null)
                throw new ArgumentNullException(nameof(Size));

            return new Rectangle(new Point(Point.X, Point.Y), new Size(Size.Width, Size.Height));
        }
    }
}