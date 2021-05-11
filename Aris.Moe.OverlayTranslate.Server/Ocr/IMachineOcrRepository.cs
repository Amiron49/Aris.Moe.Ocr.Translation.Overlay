using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public interface IMachineOcrRepository
    {
        Task<IEnumerable<ConsolidatedMachineAddressableOcr>> GetConsolidated(Guid imageId);
        Task<RawMachineOcr> Save(RawMachineOcr raw);
        Task<ConsolidatedMachineAddressableOcr> Save(ConsolidatedMachineAddressableOcr consolidated);
    }

    public enum Vote
    {
        None,
        Up,
        Down
    }
}