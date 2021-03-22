namespace Aris.Moe.OverlayTranslate.Gui.Qt5
{
    public interface ILoggingConfiguration
    {
        bool Verbose { get; set; }
        bool DebugLogging { get; set; }
        bool FileLogging { get; set; }
    }
}