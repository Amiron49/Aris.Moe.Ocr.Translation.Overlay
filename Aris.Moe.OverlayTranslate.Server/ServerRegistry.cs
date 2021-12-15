using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Image.Fetching;
using Aris.Moe.OverlayTranslate.Server.QuotaMonitoring;
using Aris.Moe.Translate;
using Lamar;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class ServerRegistry : ServiceRegistry
    {
        public ServerRegistry(ServerConfiguration baseConfiguration)
        {
            IncludeRegistry(new OverlayTranslateRegistry(baseConfiguration));
            For<IOverlayTranslateServer>().Use<OverlayTranslateServer>();
            For<IOverlayTranslateServer>().DecorateAllWith<ConcurrentTranslationRequestMitigatingServer>();
            For<IImageFactory>().Use<ImageFactory>();
            For<IImageFetcher>().Use<ImageFetcher>();
            For<IImageAnalyser>().Use<ImageAnalyser>();
            For<IImageScorer>().Use<ImageScorer>();
            For<DomainStatistics>().Use<DomainStatistics>().Singleton();

            if (baseConfiguration.Image.LogImages)
            {
                For<IImageFetcher>().DecorateAllWith<FileLoggingImageFetcher>();
                For<IOverlayTranslateServer>().DecorateAllWith<LoggingOverlayTranslateServer>();
            }

            QuotaWatching();
        }

        public void QuotaWatching()
        {
            For<IQuotaConfig>().Add<GoogleOcrQuotaConfig>();
            For<IQuotaConfig>().Add<DeeplQuotaConfig>();
            For<IQuotaMonitor>().Add<OcrQuotaGuard>();
            For<IQuotaMonitor>().Add<TranslateQuotaGuard>();
            
            For<IOcr>().DecorateAllWith<OcrQuotaGuard>();
            For<ITranslate>().DecorateAllWith<TranslateQuotaGuard>();
        }
    }

    public class ServerConfiguration : BaseConfiguration
    {
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
        public ImageConfiguration Image { get; set; } = new ImageConfiguration();
    }
}