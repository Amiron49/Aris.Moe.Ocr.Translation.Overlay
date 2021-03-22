using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public interface ITranslationRepository
    {
        Task<IEnumerable<MachineTranslation>> GetMachineTranslations(Guid imageId);
        Task<MachineTranslation> SaveMachineTranslation(int machineId, MachineTranslation machineTranslation);
    }
}