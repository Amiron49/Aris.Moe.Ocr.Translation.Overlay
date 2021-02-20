using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr
{
    public interface IOcr
    {
        Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null);
    }
}