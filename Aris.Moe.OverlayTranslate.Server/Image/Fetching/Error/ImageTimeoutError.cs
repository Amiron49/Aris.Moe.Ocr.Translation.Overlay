namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Errors
{
    public class ImageTimeoutError : FluentResults.Error
    {
        public ImageTimeoutError() : base("Failed to load the image due to a timeout. Are you sure that the image is publicly reachable?")
        {
        }
    }
}