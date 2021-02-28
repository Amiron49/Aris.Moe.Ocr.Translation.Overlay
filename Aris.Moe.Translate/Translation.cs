using System.Drawing;

namespace Aris.Moe.Translate
{
    public class Translation
    {
        public string Text { get; }
        public string OriginalText { get; }
        public int OriginalTextLength => OriginalText.Length;

        public Translation(string text, string originalText)
        {
            Text = text;
            OriginalText = originalText;
        }
    }
}