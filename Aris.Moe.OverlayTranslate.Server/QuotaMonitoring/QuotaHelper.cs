using System;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public static class QuotaHelper
    {
        public static (DateTime from, DateTime to) MonthRange(DateTime target)
        {
            var firstMomentOfMonth = new DateTime(target.Year, target.Month, 1, 0, 0, 0);
            var lastMomentOfMonth = firstMomentOfMonth.AddMonths(1).Subtract(TimeSpan.FromSeconds(1));

            return (firstMomentOfMonth, lastMomentOfMonth);
        }

        public static async Task<long> ThisMonthUnits(this IQuotaRepository repo, string type)
        {
            var (from, to) = MonthRange(DateTime.UtcNow);
            return await repo.BillableUnitsSum(@from, to, type);
        }
    }
}