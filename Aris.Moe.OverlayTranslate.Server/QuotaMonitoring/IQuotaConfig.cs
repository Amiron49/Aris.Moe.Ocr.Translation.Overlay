namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public interface IQuotaConfig
    {
        public string Type { get; }
        public double MonthlyEuroLimit { get; }
        public double EstimatedUnitCost { get; }
    }
}