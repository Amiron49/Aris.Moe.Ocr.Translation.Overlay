using System.IO;
using System.Threading.Tasks;
using FluentResults;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageFactory
    {
        public Task<Result<ImageReference>> Create(string url, Stream image);
    }
}