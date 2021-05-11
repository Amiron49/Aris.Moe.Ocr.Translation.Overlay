namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class GoogleOcrQuotaConfig : IQuotaConfig
    {
        public string Type { get; } = QuotaType.GoogleOcr;
        public double MonthlyEuroLimit => 50;
        public double EstimatedUnitCost => 1.25d / 1000;
    }
}