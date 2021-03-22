using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Ocr;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public class MachineOcrRepository : IMachineOcrRepository
    {
        private readonly OverlayTranslateServerContext _context;

        public MachineOcrRepository(OverlayTranslateServerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConsolidatedMachineAddressableOcr>> GetConsolidated(Guid imageId)
        {
            var query = _context.ConsolidatedMachineOcrs.Where(x => x.Raw.ForImage == imageId)
                .Include(x => x.Raw)
                .Include(x => x.Texts)
                .ToListAsync();

            return (await query).Select(x => x.ToBusinessModel());
        }

        public async Task<RawMachineOcr> Save(RawMachineOcr raw)
        {
            // ReSharper disable once MethodHasAsyncOverload
            var result = _context.RawMachineOcrs.Add(raw.ToModel());
            await _context.SaveChangesAsync();
            return result.Entity.ToBusinessModel();
        }

        public async Task<ConsolidatedMachineAddressableOcr> Save(ConsolidatedMachineAddressableOcr consolidated)
        {
            var result = _context.ConsolidatedMachineOcrs.Add(consolidated.ToModel());
            await _context.SaveChangesAsync();
            return result.Entity.ToBusinessModel();
        }
    }
}