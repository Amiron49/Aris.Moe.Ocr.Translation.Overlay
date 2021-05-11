using Aris.Moe.Configuration;
using Aris.Moe.Core;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Gui.Overlay.Modes;
using Aris.Moe.ScreenHelpers;
using Aris.Moe.Translate;
using Lamar;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public class GuiRegistry : ServiceRegistry
    {
        public GuiRegistry()
        {
            For<ITranslate>().Use<DeeplTranslate>();
            For<INeedConfiguration>().Add<GoogleOcr>();
            For<INeedConfiguration>().Add<DeeplTranslate>();

            For<IOcrTranslateOverlay>().Use<OcrTranslateOverlay>().Singleton();
            For<IScreenImageProvider>().Use<ScreenProvider>();
            For<IScreenInformation>().Use<WindowsScreenInformation>();
            For<IOverlay>().Use<OverlayTranslate.Gui.Overlay.Overlay>().Singleton();
            For<IOcr>().DecorateAllWith<OcrCoordinator>();
            For<ISpatialTextConsolidator>().Use<VerticalTextConsolidator>();
            Use<ProgressDisplay>().Singleton().For<IProgressDisplay>().For<IProgressDisplayGuiMode>();
        }
    }
}