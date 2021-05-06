using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.QuotaMonitoring;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class StatisticsAggregator
    {
        private readonly IEnumerable<IQuotaMonitor> _quotaMonitors;

        public StatisticsAggregator(IEnumerable<IQuotaMonitor> quotaMonitors)
        {
            _quotaMonitors = quotaMonitors;
        }

        public async Task<IEnumerable<QuotaStatistic>> Create()
        {
            var stats = _quotaMonitors.Select(async monitor =>
            {
                var thisMonth = (await monitor.GetAllThisMonth()).ToList();
                var totalMonthCost = thisMonth.Sum(x => x.EstimatedCost);
                var totalMonthUsage = thisMonth.Sum(x => x.Units);
                var averagePerDay = totalMonthCost / DateTime.UtcNow.Day;
                var averagePerDayUsage = totalMonthCost / DateTime.UtcNow.Day;
                var estimatedFinalCost = averagePerDay * 31;
                var estimatedFinalUsage = averagePerDayUsage * 31;

                return new QuotaStatistic[]
                {
                    new()
                    {
                        Cost = totalMonthCost,
                        Count = totalMonthUsage,
                        Type = monitor.Type,
                        TimeFrame = "Current Month"
                    },
                    new()
                    {
                        Cost = averagePerDay,
                        Count = averagePerDayUsage,
                        Type = monitor.Type,
                        TimeFrame = "Daily Average"
                    },
                    new()
                    {
                        Cost = estimatedFinalCost,
                        Count = estimatedFinalUsage,
                        Type = monitor.Type,
                        TimeFrame = "Month Estimate"
                    }
                };
            });

            var iHateAsyncEnumerable = new List<QuotaStatistic[]>();

            foreach (var task in stats)
                iHateAsyncEnumerable.Add(await task);

            return iHateAsyncEnumerable.SelectMany(x => x);
        }
    }

    public class QuotaStatistic
    {
        public string Type { get; set; }
        public string TimeFrame { get; set; }
        public double Count { get; set; }
        public double Cost { get; set; }
    }
}