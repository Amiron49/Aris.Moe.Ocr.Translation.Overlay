using System;
using System.IO;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageFactory : IImageFactory
    {
        private readonly IImageScorer _imageScorer;
        private readonly IImageAnalyser _imageAnalyser;

        public ImageFactory(IImageScorer imageScorer, IImageAnalyser imageAnalyser)
        {
            _imageScorer = imageScorer;
            _imageAnalyser = imageAnalyser;
        }

        public async Task<ImageReference?> Create(string url, Stream image)
        {
            if (image.Length == 0)
                return null;

            var imageInfo = await _imageAnalyser.Analyse(image);

            return new ImageReference
            {
                Id = Guid.NewGuid(),
                Info = imageInfo,
                OriginalUrl = url,
                QualityScore = await _imageScorer.Calculate(image, imageInfo)
            };
        }
    }
}