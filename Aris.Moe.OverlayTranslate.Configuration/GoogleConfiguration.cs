using System;
using System.IO;
using Aris.Moe.Configuration;

namespace Aris.Moe.OverlayTranslate.Configuration
{
    public class GoogleConfiguration : IGoogleConfiguration
    {
        public string? KeyPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, ".private", "key.json");
        public string? ProjectId { get; set; }
        public string? LocationId { get; set; } = "global";
    }
}