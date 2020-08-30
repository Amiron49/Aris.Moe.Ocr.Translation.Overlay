using System.Drawing;

namespace Aris.Moe.Ocr.Core
{
    public class SpatialText
    {
        public string Text { get; set; }
        public Rectangle Area { get; set; }

        public SpatialText(string text, Rectangle area)
        {
            Text = text;
            Area = area;
        }
    }
}