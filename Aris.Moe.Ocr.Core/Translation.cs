#nullable enable
namespace Aris.Moe.Ocr.Core
{
    public class Translation
    {
        public string Text { get; set; }
        public int OriginalTextLength { get; set; }

        public Translation(string text, int originalTextLength)
        {
            Text = text;
            OriginalTextLength = originalTextLength;
        }
    }
}