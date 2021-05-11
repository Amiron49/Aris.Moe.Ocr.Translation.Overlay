using System;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Ocr.Community;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public interface ICommunityOcrRepository
    {
        Task<CommunityAddressableOcr> Get(Guid imageId);
        Task<CommunitySpatialText?> Add(Guid imageId, CommunitySpatialText communitySpatialText);
        Task<Vote> GetVotingStatus(Guid spatialId, int userId);
        Task AddOrChangeVote(Guid spatialId, int userId, Vote vote);
    }
}