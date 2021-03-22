namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Errors
{
    public class RejectionStatusCodeError : FluentResults.Error
    {
        public RejectionStatusCodeError(int statusCode) : base(
            $"Failed to load the image due to the server rejecting the request ({statusCode}). Are you sure that the image is publicly reachable?")
        {
        }
    }
}