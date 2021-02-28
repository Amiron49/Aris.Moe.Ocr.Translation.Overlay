﻿using System;
using System.Drawing;
using System.IO;
using Aris.Moe.Configuration;
using Aris.Moe.OverlayTranslate.Core;

namespace Aris.Moe.OverlayTranslate.Gui
{
    public class Configuration : IOcrTranslateOverlayConfiguration
    {
        public Rectangle CaptureArea { get; set; } = new Rectangle(new Point(310, 126), new Size(1281, 904));
        public string? SourceLanguage { get; set; } = "ja";
        public string TargetLanguage { get; set; } = "en";
        public bool PermanentlyCacheExternalOcrResult { get; set; } = false;
        public bool PermanentlyCacheExternalTranslateResult { get; set; } = false;
        public bool DebugCapturedImage { get; set; } = false;
        public string CacheFolderRoot { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, ".cache");

        public string OcrProvider { get; set; } = "Tesseract";
        public string TranslationProvider { get; set; } = "Deepl";

        public GoogleConfiguration Google { get; set; }
        public DeeplConfiguration Deepl { get; set; }
        public LoggingConfiguration Logging { get; set; }
    }

    public class GoogleConfiguration : IGoogleConfiguration
    {
        public string? KeyPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, ".private", "key.json");
        public string? ProjectId { get; set; }
        public string? LocationId { get; set; } = "global";
    }

    public interface ILoggingConfiguration
    {
        bool Verbose { get; set; }
        bool DebugLogging { get; set; }
        bool FileLogging { get; set; }
    }

    public class LoggingConfiguration : ILoggingConfiguration
    {
        public bool Verbose { get; set; }
        public bool DebugLogging { get; set; }
        public bool FileLogging { get; set; }
    }
    
    public class DeeplConfiguration : IDeeplConfiguration
    {
        public string? ApiKey { get; set; }
        public bool UseFreeProxy { get; set; } = false;
    }
}