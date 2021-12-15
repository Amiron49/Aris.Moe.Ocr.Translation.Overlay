using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class LoggingOverlayTranslateServer : IOverlayTranslateServer
    {
        private readonly IOverlayTranslateServer _overlayTranslateServerImplementation;
        private readonly ILogger<LoggingOverlayTranslateServer> _logger;

        public LoggingOverlayTranslateServer(IOverlayTranslateServer overlayTranslateServerImplementation, ILogger<LoggingOverlayTranslateServer> logger)
        {
            _overlayTranslateServerImplementation = overlayTranslateServerImplementation;
            _logger = logger;
        }

        public Task<Result<OcrTranslateResponse?>> UrlLookup(IUrlLookup request)
        {
            return _overlayTranslateServerImplementation.UrlLookup(request);
        }

        public Task<Result<OcrTranslateResponse?>> HashLookup(IHashLookup request)
        {
            return _overlayTranslateServerImplementation.HashLookup(request);
        }

        public Task<Result<OcrTranslateResponse?>> TranslatePublic(PublicOcrTranslationRequest request)
        {
            _logger.LogInformation($"Got hit with request for translating: {request.ImageUrl}");
            
            return _overlayTranslateServerImplementation.TranslatePublic(request);
        }
    }
}