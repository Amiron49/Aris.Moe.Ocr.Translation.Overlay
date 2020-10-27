using System;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.DependencyInjection;
using Lamar;

namespace Aris.Moe.Ocr.Overlay.Translate.Cli
{
    public class OverlayTranslateCliRegistry : ServiceRegistry
    {
        public OverlayTranslateCliRegistry(Config configuration)
        {
            IncludeRegistry<OverlayTranslateRegistry>();
            For<Action<string>>().Use(Console.WriteLine);

            For<IOcrTranslateOverlayConfiguration>().Use(configuration);
            For<IGoogleConfiguration>().Use(configuration);
        }
    }
}