using Aris.Moe.Configuration;
using Aris.Moe.Core;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes;
using Aris.Moe.ScreenHelpers;
using Aris.Moe.Translate;
using Lamar;

namespace Aris.Moe.Ocr.Overlay.Translate.DependencyInjection
{
    public class OverlayTranslateRegistry : ServiceRegistry
    {
        public OverlayTranslateRegistry()
        {
            For<ITranslate>().Use<GoogleTranslate>();
            For<IOcr>().Use<TesseractOcr>();
            For<INeedConfiguration>().Add<GoogleTranslate>();
            //For<INeedConfiguration>().Add<GoogleOcr>();

            For<IOcrTranslateOverlay>().Use<OcrTranslateOverlay>().Singleton();
            For<IScreenImageProvider>().Use<ScreenProvider>();
            For<IScreenInformation>().Use<WindowsScreenInformation>();
            For<IOverlay>().Use<Overlay.Overlay>().Singleton();
            For<IOcr>().DecorateAllWith<OcrCoordinator>();
            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
            For<IOcr>().DecorateAllWith<OcrDebugCache>();
            For<ITranslate>().DecorateAllWith<TranslateDebugCache>();
            Use<ProgressDisplay>().Singleton().For<IProgressDisplay>().For<IProgressDisplayGuiMode>();
        }
    }
}