using System;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.DependencyInjection;
using Lamar;
using Serilog;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    public class OverlayTranslateGuiRegistry : ServiceRegistry
    {
        public OverlayTranslateGuiRegistry(Config configuration, ILogger logger)
        {
            IncludeRegistry<OverlayTranslateRegistry>();
            For<Action<string>>().Use(Console.WriteLine);

            For<ILogger>().Use(logger);
            For<IOcrTranslateOverlayConfiguration>().Use(configuration);
            For<IGoogleConfiguration>().Use(configuration).Singleton();
        }
    }
}