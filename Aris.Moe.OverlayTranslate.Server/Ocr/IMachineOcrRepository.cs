using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Ocr.Community;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public interface IMachineOcrRepository
    {
        Task<IEnumerable<ConsolidatedMachineAddressableOcr>> GetConsolidated(Guid imageId);
        Task<RawMachineOcr> Save(RawMachineOcr raw);
        Task<ConsolidatedMachineAddressableOcr> Save(ConsolidatedMachineAddressableOcr consolidated);
    }
    
    public interface ICommunityOcrRepository
    {
        Task<CommunityAddressableOcr> Get(Guid imageId);
        Task<CommunitySpatialText?> Add(Guid imageId, CommunitySpatialText communitySpatialText);
        Task<Vote> GetVotingStatus(Guid spatialId, int userId);
        Task AddOrChangeVote(Guid spatialId, int userId, Vote vote);
    }

    public enum Vote
    {
        None,
        Up,
        Down
    }
}