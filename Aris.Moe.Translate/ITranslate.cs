using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.Translate
{
    public interface ITranslate
    {
        Task<IEnumerable<Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null);
    }
}