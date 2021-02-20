using System.Collections.Generic;
using System.Threading.Tasks;
using Aris.Moe.Ocr;

namespace Aris.Moe.Translate
{
    public interface ITranslate
    {
        Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null);
    }
}