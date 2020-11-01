using Qml.Net;
using Qml.Net.Runtimes;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    public class UserGui
    {
        public static int Run()
        {
            RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();
            
            QQuickStyle.SetStyle("Material");

            QCoreApplication.SetAttribute(ApplicationAttribute.EnableHighDpiScaling, true);
            
            using var app = new QGuiApplication();
            using var engine = new QQmlApplicationEngine();


            Qml.Net.Qml.RegisterType<SettingsModel>("Aris.Moe.Ocr.Overlay.Translate.Gui", 1, 1);
            Qml.Net.Qml.RegisterType<ControlsModel>("Aris.Moe.Ocr.Overlay.Translate.Gui", 1, 1);

            engine.Load("Main.qml");

            return app.Exec();
        }
    }
}