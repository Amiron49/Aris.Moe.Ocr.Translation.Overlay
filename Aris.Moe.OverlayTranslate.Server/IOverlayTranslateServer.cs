using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using FluentResults;

namespace Aris.Moe.OverlayTranslate.Server
{
    public interface IOverlayTranslateServer
    {
        public Task<Result<OcrTranslateResponse?>> UrlLookup(IUrlLookup request);
        public Task<Result<OcrTranslateResponse?>> HashLookup(IHashLookup request);
        public Task<Result<OcrTranslateResponse?>> TranslatePublic(PublicOcrTranslationRequest request);
    }
}