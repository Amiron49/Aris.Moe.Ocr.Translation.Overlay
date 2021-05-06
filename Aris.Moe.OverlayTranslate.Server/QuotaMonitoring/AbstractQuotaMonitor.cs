using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class AbstractQuotaMonitor : IQuotaMonitor
    {
        public string Type { get; }
        
        private readonly IQuotaRepository _quotaRepository;
        private readonly IQuotaConfig _config;

        public AbstractQuotaMonitor(IQuotaRepository quotaRepository, IEnumerable<IQuotaConfig> configs, string type)
        {
            _quotaRepository = quotaRepository;
            _config = configs.Single(x => x.Type == type);
            Type = type;
        }

        public async Task VerifyQuotaSpace(long unitUsage)
        {
            var unitsThisMonth = await _quotaRepository.ThisMonthUnits(_config.Type);
            var currentCost = CalculateCost(unitsThisMonth);

            if (currentCost >= _config.MonthlyEuroLimit)
                throw new QuotaExceededException(_config.Type);

            var combinedUnits = unitsThisMonth + unitUsage;
            var estimatedTotalCost = CalculateCost(combinedUnits);

            if (estimatedTotalCost >= _config.MonthlyEuroLimit)
                throw new QuotaWouldBeExceededException(_config.Type, unitsThisMonth, estimatedTotalCost - _config.MonthlyEuroLimit);
        }

        public async Task UseQuota(long amount)
        {
            await _quotaRepository.Add(new QuotaUsage
            {
                Time = DateTime.UtcNow,
                Units = amount,
                EstimatedCost = CalculateCost(amount),
                Type = _config.Type
            });
        }

        public Task<IEnumerable<QuotaUsage>> GetAllThisMonth()
        {
            var (from, to) = QuotaHelper.MonthRange(DateTime.UtcNow);
            return _quotaRepository.GetAll(from, to, _config.Type);
        }

        private double CalculateCost(long units)
        {
            return units * _config.EstimatedUnitCost;
        }
    }
}