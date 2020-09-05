using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface ITranslate
    {
        Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null);
    }
}