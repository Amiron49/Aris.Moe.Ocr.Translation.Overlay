using System;
using System.IO;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Core;
using Lamar;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Aris.Moe.OverlayTranslate.Gui
{
    internal class Program
    {
        public static IContainer Services = null!;

        private static async Task<int> Main(string[] args)
        {
            var fileLoggingLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "logs", "emergency.log");
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(fileLoggingLocation)
                .CreateLogger();

            try
            {
                var config = BuildConfiguration(args);

                Services = new Container(new OverlayTranslateGuiRegistry(config));

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

        private static Configuration BuildConfiguration(string[] args)
        {
            var configurationProvider =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Debug.json", true)
                    .AddCommandLine(args).Build();

            var config = new Configuration();
            configurationProvider.Bind(config);
            return config;
        }
    }
}