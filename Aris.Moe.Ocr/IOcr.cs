using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr
{
    public interface IOcr
    {
        Task<(IEnumerable<ISpatialText> Texts, string Language)> Ocr(Stream image, string? inputLanguage = null);
    }
}