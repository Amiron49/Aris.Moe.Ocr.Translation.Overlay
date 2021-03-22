using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Ocr;
using Aris.Moe.OverlayTranslate.Server.Ocr.Community;

namespace Aris.Moe.OverlayTranslate.Server
{
    public interface ICommunityOcrRepository    
    {
        Task<List<AddressableOcr<CommunitySpatialText>>> Get(string hash);
        Task<IEnumerable<ImageReference>> GetAll(ImageInfo info);
        Task<ImageReference> Save(Stream image, ImageInfo info);
    }
}