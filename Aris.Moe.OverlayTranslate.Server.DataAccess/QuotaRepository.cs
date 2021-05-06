using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.QuotaMonitoring;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public class QuotaRepository : IQuotaRepository
    {
        private readonly OverlayTranslateServerContext _context;

        public QuotaRepository(OverlayTranslateServerContext context)
        {
            _context = context;
        }
        
        public async Task Add(QuotaUsage quotaUsage)
        {
            _context.QuotaUsages.Add(new QuotaUsageModel
            {
                Time = quotaUsage.Time,
                Type = quotaUsage.Type,
                Units = quotaUsage.Units,
                EstimatedCost = quotaUsage.EstimatedCost
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuotaUsage>> GetAll(DateTime from, DateTime to, string? type = null)
        {
            var query = GetAllQuery(@from, to, type);
            return await query.ToListAsync();
        }

        private IQueryable<QuotaUsage> GetAllQuery(DateTime from, DateTime to, string? type)
        {
            var query = _context.QuotaUsages.AsQueryable().Where(x => x.Time >= @from && x.Time <= to);
            if (type != null)
                query = query.Where(x => x.Type == type);
            
            return query;
        }

        public async Task<long> BillableUnitsSum(DateTime from, DateTime to, string? type = null)
        {
            var query = GetAllQuery(from, to, type);
            return await query.SumAsync(x => x.Units);
        }
    }

    public class QuotaUsageModel : QuotaUsage
    {
        public int? Id { get; set; }
    }
}