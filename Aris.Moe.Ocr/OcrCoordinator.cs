using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Ocr
{
    public class OcrCoordinator : IOcr
    {
        private readonly IOcr _decorated;
        private readonly ISpatialTextConsolidator _spatialTextConsolidator;
        private readonly ILogger<OcrCoordinator> _logger;

        public OcrCoordinator(IOcr decorated, ISpatialTextConsolidator spatialTextConsolidator, ILogger<OcrCoordinator> logger)
        {
            _decorated = decorated;
            _spatialTextConsolidator = spatialTextConsolidator;
            _logger = logger;
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            var googleOcrResult = await _decorated.Ocr(image, inputLanguage);

            var consolidated = _spatialTextConsolidator.Consolidate(googleOcrResult);

            foreach (var spatialText in consolidated)
                _logger.LogInformation(spatialText.Text);
            
            return consolidated;
        }
    }
}