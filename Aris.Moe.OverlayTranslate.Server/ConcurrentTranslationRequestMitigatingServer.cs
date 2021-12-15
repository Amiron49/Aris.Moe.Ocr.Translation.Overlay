using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server
{
    /// <summary>
    /// TODO remove once all users have the latest Extension. Or at least disable it
    /// </summary>
    public class ConcurrentTranslationRequestMitigatingServer: IOverlayTranslateServer
    {
        private readonly IOverlayTranslateServer _overlayTranslateServerImplementation;
        private readonly ILogger<ConcurrentTranslationRequestMitigatingServer> _logger;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();
        private static readonly ConcurrentDictionary<string, bool> QuickLookup = new();
        
        public ConcurrentTranslationRequestMitigatingServer(IOverlayTranslateServer overlayTranslateServerImplementation, ILogger<ConcurrentTranslationRequestMitigatingServer> logger)
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

        public async Task<Result<OcrTranslateResponse?>> TranslatePublic(PublicOcrTranslationRequest request)
        {
            var worked = QuickLookup.TryGetValue(request.ImageUrl, out var alreadyKnown);

            if (worked && alreadyKnown)
                return await _overlayTranslateServerImplementation.TranslatePublic(request);
            
            var result = await HandleConcurrencyHell(request);

            QuickLookup.TryAdd(request.ImageUrl, true);
            
            return result;
        }

        private async Task<Result<OcrTranslateResponse?>> HandleConcurrencyHell(PublicOcrTranslationRequest request)
        {
            var key = request.ImageUrl;

            var hadASemaphore = Semaphores.TryGetValue(key, out var existingSemaphore);
            if (hadASemaphore && existingSemaphore != null)
            {
                await existingSemaphore.WaitAsync();
                existingSemaphore.Release();
                return await _overlayTranslateServerImplementation.TranslatePublic(request);
            }

            var semaphore = new SemaphoreSlim(0, 1);

            var imResponsibleForTheImage = Semaphores.TryAdd(key, semaphore);

            if (imResponsibleForTheImage)
            {
                try
                {
                    return await _overlayTranslateServerImplementation.TranslatePublic(request);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            var gotSemaphoreThatManagedToSnipeOwnAdd = Semaphores.TryGetValue(key, out var snipingSemaphore);

            //If all of this fails. fall back on the normal implementation anyway.
            if (!gotSemaphoreThatManagedToSnipeOwnAdd || snipingSemaphore == null)
                return await _overlayTranslateServerImplementation.TranslatePublic(request);

            await snipingSemaphore.WaitAsync();
            snipingSemaphore.Release();
            return await _overlayTranslateServerImplementation.TranslatePublic(request);
        }
    }
}