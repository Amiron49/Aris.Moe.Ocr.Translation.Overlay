using System;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Lamar;
using Serilog;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    internal class Program
    {
        public static IContainer Services = null!;

        private static async Task<int> Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var config = new Config();
                Services = new Container(new OverlayTranslateGuiRegistry(config, Log.Logger));

                using (Services)
                {
                    var overlay = Services.GetInstance<IOverlay>();
                    
                    await overlay.Init();
                    overlay.ShowOverlay();
                    return UserGui.Run();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }

            Log.CloseAndFlush();
            return 0;
        }
    }
}