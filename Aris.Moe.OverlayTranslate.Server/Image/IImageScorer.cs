using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public interface IImageScorer
    {
        Task<double> Calculate(Stream stream, ImageInfo info);
    }

    public class ImageScorer : IImageScorer
    {
        public Task<double> Calculate(Stream stream, ImageInfo info)
        {
            var modifier = 1.0;

            if (info.MimeType.Contains("jpg"))
                modifier = 0.8;

            var score = info.Height * info.Width * modifier;
            
            return Task.FromResult(score);
        }
    }
}