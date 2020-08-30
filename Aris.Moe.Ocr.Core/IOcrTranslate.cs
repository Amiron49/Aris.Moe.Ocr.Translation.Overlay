using System.Collections.Generic;

namespace Aris.Moe.Ocr.Core
{
    public interface IOcrTranslate
    {
        IEnumerable<Translation> Translate(object image, string? targetLanguage = "en");
    }
}