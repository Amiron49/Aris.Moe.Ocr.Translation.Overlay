using System;
using System.IO;
using Aris.Moe.Configuration;

namespace Aris.Moe.OverlayTranslate.Configuration
{
    public class GoogleConfiguration : IGoogleConfiguration
    {
        public string? Key { get; set; }
        public string? KeyPath { get; set; }
        public string? ProjectId { get; set; }
        public string? LocationId { get; set; } = "global";
    }
}