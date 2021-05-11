using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Translate;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class TranslateQuotaGuard : AbstractQuotaMonitor, ITranslate
    {
        private readonly ITranslate _translateImplementation;

        public TranslateQuotaGuard(ITranslate translateImplementation, IQuotaRepository quotaRepository, IEnumerable<IQuotaConfig> configs) : base(quotaRepository, configs, QuotaType.DeeplTranslate)
        {
            _translateImplementation = translateImplementation;
        }

        public async Task<IEnumerable<Translate.Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var originalsAsList = originals.ToList();
            var unitUsage = originalsAsList.Sum(x => x.Length);
            await VerifyQuotaSpace(unitUsage);
            await UseQuota(unitUsage);
            return await _translateImplementation.Translate(originalsAsList, targetLanguage, inputLanguage);
        }
    }
}