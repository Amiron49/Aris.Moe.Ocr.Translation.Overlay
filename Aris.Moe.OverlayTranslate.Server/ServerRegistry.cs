using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Image.Fetching;
using Lamar;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class ServerRegistry : ServiceRegistry
    {
        public ServerRegistry(BaseConfiguration baseConfiguration)
        {
            IncludeRegistry(new OverlayTranslateRegistry(baseConfiguration));
            For<IOverlayTranslateServer>().Use<OverlayTranslateServer>();
            For<IImageFactory>().Use<ImageFactory>();
            For<IImageFetcher>().Use<ImageFetcher>();
            For<IImageAnalyser>().Use<ImageAnalyser>();
            For<IImageScorer>().Use<ImageScorer>();
            For<DomainStatistics>().Use<DomainStatistics>().Singleton();
        }
    }
}