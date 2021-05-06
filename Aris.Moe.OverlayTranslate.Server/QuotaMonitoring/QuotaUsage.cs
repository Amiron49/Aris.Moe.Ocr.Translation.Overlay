using System;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class QuotaUsage
    {
        public string Type { get; init; }
        public DateTime Time { get; init; }
        public double EstimatedCost { get; init; }
        public long Units { get; init; }
    }
}