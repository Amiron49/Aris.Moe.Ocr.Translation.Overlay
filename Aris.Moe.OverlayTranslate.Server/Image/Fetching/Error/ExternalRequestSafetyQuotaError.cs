namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class ExternalRequestSafetyQuotaError : CorrelatedError
    {
        public ExternalRequestSafetyQuotaError(string domain, int max) : base(
            $"External request rate towards the domain {domain} has exceeded {max} requests per second.")
        {
        }
    }
}