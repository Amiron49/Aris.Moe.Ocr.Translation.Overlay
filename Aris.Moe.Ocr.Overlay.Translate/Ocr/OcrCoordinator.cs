using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class OcrCoordinator : IOcr
    {
        private readonly IOcr _decorated;
        private readonly ISpatialTextConsolidator _spatialTextConsolidator;

        public OcrCoordinator(IOcr decorated, ISpatialTextConsolidator spatialTextConsolidator)
        {
            _decorated = decorated;
            _spatialTextConsolidator = spatialTextConsolidator;
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            var googleOcrResult = await _decorated.Ocr(image, inputLanguage);

            var consolidated = _spatialTextConsolidator.Consolidate(googleOcrResult);

            return consolidated;
        }
    }
}