using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.Ocr;
using Aris.Moe.ScreenHelpers;
using Lamar;

namespace Aris.Moe.Ocr.Overlay.Translate.DependencyInjection
{
    public class OverlayTranslateRegistry : ServiceRegistry
    {
        public OverlayTranslateRegistry()
        {
            For<IOcrTranslateOverlay>().Use<OcrTranslateOverlay>().Singleton();
            For<IScreenImageProvider>().Use<ScreenProvider>();
            For<IScreenInformation>().Use<WindowsScreenInformation>();
            For<IOverlay>().Use<Overlay.Overlay>().Singleton();
            For<IOcr>().Use<GoogleOcr>();
            For<IOcr>().DecorateAllWith<OcrCoordinator>();
            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
            For<ITranslate>().Use<GoogleTranslate>();
            For<IOcr>().DecorateAllWith<OcrDebugCache>();
            For<ITranslate>().DecorateAllWith<TranslateDebugCache>();
        }
    }
}