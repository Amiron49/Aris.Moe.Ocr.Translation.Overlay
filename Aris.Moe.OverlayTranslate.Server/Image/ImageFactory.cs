using System;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageFactory : IImageFactory
    {
        private readonly IImageScorer _imageScorer;
        private readonly IImageAnalyser _imageAnalyser;
        private readonly ILogger<IImageFactory> _logger;

        public ImageFactory(IImageScorer imageScorer, IImageAnalyser imageAnalyser, ILogger<IImageFactory> logger)
        {
            _imageScorer = imageScorer;
            _imageAnalyser = imageAnalyser;
            _logger = logger;
        }

        public async Task<Result<ImageReference>> Create(string url, Stream image)
        {
            if (image.Length == 0)
                return Result.Fail(new ImageIsEmptyError());

            image.Position = 0;
            ImageInfo imageInfo;
            try
            {
                imageInfo = await _imageAnalyser.Analyse(image);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't analyze image");
                return Result.Fail(new ImageAnalyzerError().CausedBy(e));
            }
            
            image.Position = 0;
            var score = await _imageScorer.Calculate(image, imageInfo);
            
            return Result.Ok(new ImageReference(Guid.NewGuid(), imageInfo, url, score));
        }
    }

    public class ImageIsEmptyError : FluentResults.Error
    {
        public ImageIsEmptyError(): base("Image length received by the server was 0")
        {
        }
    }
    
    public class ImageAnalyzerError : CorrelatedError
    {
        public ImageAnalyzerError(): base("Image couldn't be analyzed. Might be an unsupported image format or the request to the image got something that isn't an image.")
        {
        }
    }
}