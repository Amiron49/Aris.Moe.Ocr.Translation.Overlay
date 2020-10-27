using System;
using System.Drawing;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.DependencyInjection;
using Lamar;
using Qml.Net;
using Qml.Net.Runtimes;
using Serilog;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    class Program
    {
        public static IContainer Services = null!;

        static async Task<int> Main(string[] args)
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
                    var gui = new UserGui();
                    var overlay = Services.GetInstance<IOverlay>();

                    await overlay.Init();
                    overlay.ShowOverlay();
                    return gui.Run();
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

    public class UserGui
    {
        public int Run()
        {
            RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();
            QQuickStyle.SetStyle("Material");

            using (var app = new QGuiApplication())
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    // TODO: Register your .NET types.
                    Qml.Net.Qml.RegisterType<SettingsModel>("Aris.Moe.Ocr.Overlay.Translate.Gui", 1, 1);
                    Qml.Net.Qml.RegisterType<ControlsModel>("Aris.Moe.Ocr.Overlay.Translate.Gui", 1, 1);

                    engine.Load("Main.qml");

                    return app.Exec();
                }
            }
        }
    }

    public class OverlayTranslateGuiRegistry : ServiceRegistry
    {
        public OverlayTranslateGuiRegistry(Config configuration, ILogger logger)
        {
            IncludeRegistry<OverlayTranslateRegistry>();
            For<Action<string>>().Use(Console.WriteLine);

            For<ILogger>().Use(logger);
            For<IOcrTranslateOverlayConfiguration>().Use(configuration);
            For<IGoogleConfiguration>().Use(configuration);
        }
    }

    public class Config : IGoogleConfiguration, IOcrTranslateOverlayConfiguration
    {
        public string? KeyPath { get; set; } = @"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private\key.json";
        public Rectangle ScreenArea { get; set; } = new Rectangle(new Point(310, 126), new Size(1281, 904));
        public string? SourceLanguage { get; set; } = "ja";
        public string TargetLanguage { get; set; } = "en";
        public bool PermanentlyCacheExternalOcrResult { get; set; } = false;
        public bool PermanentlyCacheExternalTranslateResult { get; set; } = false;
        public string CacheFolderRoot { get; set; } = @"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private";
    }
}