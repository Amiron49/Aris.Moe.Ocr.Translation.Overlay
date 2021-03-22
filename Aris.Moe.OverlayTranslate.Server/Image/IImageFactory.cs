using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageFactory
    {
        public Task<ImageReference?> Create(string url, Stream image);
    }
}