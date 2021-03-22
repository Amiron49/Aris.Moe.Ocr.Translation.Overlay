using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageReferenceRepository
    {
        Task<ImageReference?> Get(byte[] hash);
        Task<ImageReference?> Get(Guid id);
        Task<IEnumerable<ImageReference>> GetAll(ImageInfo info);
        Task<ImageReference> Save(ImageReference imageReference);
    }
}