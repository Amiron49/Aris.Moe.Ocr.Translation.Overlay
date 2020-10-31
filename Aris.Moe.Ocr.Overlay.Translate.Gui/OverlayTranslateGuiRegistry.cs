using System;
using System.IO;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.DependencyInjection;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Aris.Moe.Ocr.Overlay.Translate.Gui
{
    public class OverlayTranslateGuiRegistry : ServiceRegistry
    {
        public OverlayTranslateGuiRegistry(Configuration configuration)
        {
            var logger = CreateLogger(configuration.Logging);
            
            IncludeRegistry<OverlayTranslateRegistry>();
            
            this.AddLogging(builder => builder.AddSerilog(logger));

            For<IOcrTranslateOverlayConfiguration>().Use(configuration).Singleton();
            For<IGoogleConfiguration>().Use(configuration.Google).Singleton();
        }
        
        private static ILogger CreateLogger(ILoggingConfiguration configuration) {
            var builder =  new LoggerConfiguration();

            if (configuration.Verbose)
                builder = builder.MinimumLevel.Debug();
            else
                builder = builder.MinimumLevel.Information();

            if (configuration.FileLogging)
            {
                var fileLoggingLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "logs", "regular.log");
                builder = builder.WriteTo.File(fileLoggingLocation);
            }

            if (configuration.DebugLogging)
                builder = builder.WriteTo.Debug();

            return builder.CreateLogger();
        }
    }
}