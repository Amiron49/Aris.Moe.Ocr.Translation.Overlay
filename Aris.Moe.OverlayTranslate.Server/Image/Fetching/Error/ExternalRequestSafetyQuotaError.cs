namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Errors
{
    public class ExternalRequestSafetyQuotaError : FluentResults.Error
    {
        public ExternalRequestSafetyQuotaError(string domain, int max) : base(
            $"External request rate towards the domain {domain} has exceeded {max} requests per second.")
        {
        }
    }
}