using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public interface IQuotaRepository
    {
        public Task Add(QuotaUsage quotaUsage);
        public Task<IEnumerable<QuotaUsage>> GetAll(DateTime from, DateTime to, string? type = null);
        public Task<long> BillableUnitsSum(DateTime @from, DateTime to, string? type = null);
    }
}