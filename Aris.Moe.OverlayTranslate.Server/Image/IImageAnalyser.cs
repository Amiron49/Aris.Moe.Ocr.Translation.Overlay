using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageAnalyser
    {
        Task<ImageInfo> Analyse(Stream imageStream);
    }
}