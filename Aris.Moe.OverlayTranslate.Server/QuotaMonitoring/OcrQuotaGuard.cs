using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.Ocr;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class OcrQuotaGuard : AbstractQuotaMonitor, IOcr
    {
        private readonly IOcr _ocrImplementation;

        public OcrQuotaGuard(IOcr ocrImplementation, IQuotaRepository quotaRepository, IEnumerable<IQuotaConfig> configs) : base(quotaRepository, configs, QuotaType.GoogleOcr)
        {
            _ocrImplementation = ocrImplementation;
        }

        public async Task<(IEnumerable<ISpatialText> Texts, string Language)> Ocr(Stream image, string? inputLanguage = null)
        {
            const int unitUsage = 1;
            await VerifyQuotaSpace(unitUsage);
            await UseQuota(unitUsage);
            var result = await _ocrImplementation.Ocr(image, inputLanguage);
            return result;
        }
    }
}