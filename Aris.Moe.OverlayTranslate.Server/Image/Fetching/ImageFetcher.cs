using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class ImageFetcher : IImageFetcher
    {
        private const int MaxDomainRequestsPerSecond = 20;
        private const long MinContentLength = 2000;
        private const long MaxContentLength = 10485760;
        
        private readonly ILogger<ImageFetcher> _logger;
        private readonly DomainStatistics _domainStatistics;

        public ImageFetcher(ILogger<ImageFetcher> logger, DomainStatistics domainStatistics)
        {
            _logger = logger;
            _domainStatistics = domainStatistics;
        }

        public async Task<Result<Stream>> Get(string url)
        {
            var domainStatistics = _domainStatistics.GetStats(url);
            var requestRatePerSecond = domainStatistics.RequestsPerSecond();
            if (requestRatePerSecond >= MaxDomainRequestsPerSecond)
                return Result.Fail<Stream>(new ExternalRequestSafetyQuotaError(domainStatistics.Domain, MaxDomainRequestsPerSecond));
            
            using var client = new HttpClient()
            {
                DefaultRequestHeaders =
                {
                    {"X-Report-Abuse", "honyaku-server-abuse@aris.moe"},
                },
                Timeout = TimeSpan.FromSeconds(20)
            };

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                var error = GetResponseError(domainStatistics.Domain, response);

                if (error != null)
                    return Result.Fail<Stream>(error);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return Result.Fail<Stream>(new ImageTimeoutError());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to fetch image due to an unknown error");
                return Result.Fail<Stream>(new UnknownImageFetchingError().CausedBy(e));
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            await using var streamLengthGuard = new StreamLengthGuard(contentStream, domainStatistics.Domain, MaxContentLength);
            
            var memoryStream = new MemoryStream();
            await streamLengthGuard.CopyToAsync(memoryStream);

            return Result.Ok<Stream>(memoryStream);
        }

        private FluentResults.Error? GetResponseError(string domain, HttpResponseMessage responseMessage)
        {
            var responseStatusCode = (int)responseMessage.StatusCode;
            
            switch (responseStatusCode)
            {
                case >= 400 and < 500:
                    return new RejectionStatusCodeError(responseStatusCode);
                case >= 500:
                    return new ExternalServerError();
            }

            var contentLength = GetContentLength(domain, responseMessage); 
            
            return contentLength switch
            {
                < MinContentLength => new ImageTooSmallError(contentLength.Value, MinContentLength),
                > MaxContentLength => new ImageTooBigError(contentLength.Value, MinContentLength),
                _ => null
            };
        }

        private long? GetContentLength(string domain, HttpResponseMessage responseMessage)
        {
            var contentLength = responseMessage.Content.Headers.GetValues("Content-Length").FirstOrDefault();

            if (string.IsNullOrEmpty(contentLength))
            {
                _logger.LogWarning($"No content-length header from domain '{domain}'");
                return null;
            }

            var parseSuccess = long.TryParse(contentLength, out var contentLengthAsLong);

            if (parseSuccess) 
                return contentLengthAsLong;
            
            _logger.LogWarning($"Couldn't parse content-length header from domain '{domain}'. '{contentLength}'");
            return null;
        }
    }
}