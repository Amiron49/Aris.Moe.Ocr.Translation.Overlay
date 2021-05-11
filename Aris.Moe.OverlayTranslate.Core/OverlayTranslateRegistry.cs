using Aris.Moe.Configuration;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.Translate;
using Lamar;

namespace Aris.Moe.OverlayTranslate.Core
{
    public class OverlayTranslateRegistry : ServiceRegistry
    {
        public OverlayTranslateRegistry(BaseConfiguration baseConfiguration)
        {
            For<IGoogleConfiguration>().Use(baseConfiguration.Google);
            For<IDeeplConfiguration>().Use(baseConfiguration.Deepl);
            
            For<ITranslate>().Use<DeeplTranslate>();
            For<IOcr>().Use<GoogleOcr>();

            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
        }
    }
}