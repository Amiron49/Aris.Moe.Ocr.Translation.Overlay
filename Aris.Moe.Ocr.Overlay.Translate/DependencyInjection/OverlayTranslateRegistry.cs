using System;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Lamar;

namespace Aris.Moe.Ocr.Overlay.Translate.DependencyInjection
{
    public class OverlayTranslateRegistry : ServiceRegistry
    {
        public OverlayTranslateRegistry()
        {
            For<IOcrTranslateOverlay>().Use<OcrTranslateOverlay>().Singleton();
            For<IScreenImageProvider>().Use<ScreenProvider>();
            For<IOverlay>().Use<Overlay>().Singleton();
            For<IOcr>().Use<GoogleOcr>();
            For<IOcr>().DecorateAllWith<OcrCoordinator>();
            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
            For<ITranslate>().Use<GoogleTranslate>();
            For<IOcr>().DecorateAllWith<OcrDebugCache>();
            For<ITranslate>().DecorateAllWith<TranslateDebugCache>();
        }
    }
}