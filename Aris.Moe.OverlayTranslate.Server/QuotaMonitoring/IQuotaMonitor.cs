using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public interface IQuotaMonitor
    {
        public string Type { get; }
        Task VerifyQuotaSpace(long amount);
        Task UseQuota(long amount);
        Task<IEnumerable<QuotaUsage>> GetAllThisMonth();
    }
}