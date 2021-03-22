using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public class RectangleModel
    {
        public PointModel TopLeft { get; set; }
        public PointModel BottomRight { get; set; }
    }

    public static class RectangleExtension
    {
        public static RectangleModel ToModel(this Rectangle rectangle)
        {
            return new RectangleModel
            {
                TopLeft = new PointModel
                {
                    X = rectangle.X,
                    Y = rectangle.Y
                },
                BottomRight = new PointModel
                {
                    X = rectangle.Right,
                    Y = rectangle.Bottom
                }
            };
        }
        
        public static Rectangle ToRectangle(this RectangleModel rectangle)
        {
            var x = rectangle.TopLeft.X;
            var y = rectangle.TopLeft.Y;
            var width = rectangle.BottomRight.X - x;
            var height = rectangle.BottomRight.Y - y;
            return new Rectangle(x, y, width, height);
        }
    }
}