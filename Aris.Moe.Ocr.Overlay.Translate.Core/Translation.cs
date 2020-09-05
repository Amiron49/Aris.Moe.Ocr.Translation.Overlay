using System.Drawing;

#nullable enable
namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public class Translation
    {
        public string Text { get; set; }
        public int OriginalTextLength { get; set; }
        
        public Rectangle Area { get; set; }

        public Translation(string text, int originalTextLength, Rectangle area)
        {
            Text = text;
            OriginalTextLength = originalTextLength;
            Area = area;
        }
    }
}