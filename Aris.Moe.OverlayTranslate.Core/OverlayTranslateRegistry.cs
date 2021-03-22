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
            For<INeedConfiguration>().Add<GoogleTranslate>();
            //For<INeedConfiguration>().Add<GoogleOcr>();
            
            For<ITranslate>().Use<GoogleTranslate>();
            For<ITranslate>().DecorateAllWith<TranslateDebugCache>();
            For<IOcr>().Use<GoogleOcr>();
            For<IOcr>().DecorateAllWith<OcrDebugCache>();

            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
        }
    }
}