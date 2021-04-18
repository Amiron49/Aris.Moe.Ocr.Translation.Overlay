using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Aris.Moe.OverlayTranslate.Server.Image;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public class ImageReferenceRepository : IImageReferenceRepository
    {
        private readonly OverlayTranslateServerContext _context;

        public ImageReferenceRepository(OverlayTranslateServerContext context)
        {
            _context = context;
        }

        public async Task<ImageReference?> Get(string url)
        {
            var hash = url.Sha256();
            var singleOrDefaultAsync = await _context.Images.SingleOrDefaultAsync(x => x.Urls.Any(imageUrl => imageUrl.UrlHash == hash));
            return singleOrDefaultAsync?.ToBusinessModel();
        }

        public async Task<ImageReference?> Get(byte[] hash)
        {
            var singleOrDefaultAsync = await _context.Images.SingleOrDefaultAsync(x => x.Info.Sha256Hash == hash);
            return singleOrDefaultAsync?.ToBusinessModel();
        }

        public async Task<ImageReference?> Get(Guid id)
        {
            return (await _context.Images.SingleOrDefaultAsync(x => x.Id == id)).ToBusinessModel();
        }
        
        public async Task AddUrl(Guid id, string url)
        {
            var newUrl = new ImageUrl
            {
                OriginalUrl = url,
                UrlHash = url.Sha256(),
                ImageReferenceId = id
            };
            
            _context.Urls.Add(newUrl);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ImageReference>> GetAll(ImageInfo info)
        {
            return (await _context.Images.Where(x => x.Info.AverageHash == info.AverageHash).ToListAsync()).Select(x => x.ToBusinessModel());
        }

        public async Task<ImageReference> Save(ImageReference imageReference)
        {
            // ReSharper disable once MethodHasAsyncOverload
            var result = _context.Images.Add(imageReference.ToModel());
            await _context.SaveChangesAsync();
            return result.Entity.ToBusinessModel();
        }
    }
}