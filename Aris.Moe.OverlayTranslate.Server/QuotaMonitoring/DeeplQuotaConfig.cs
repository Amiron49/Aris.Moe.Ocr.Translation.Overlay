namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class DeeplQuotaConfig : IQuotaConfig
    {
        public string Type { get; } = QuotaType.DeeplTranslate;
        public double MonthlyEuroLimit => 25;
        public double EstimatedUnitCost => 20d / 1000000;
    }
}