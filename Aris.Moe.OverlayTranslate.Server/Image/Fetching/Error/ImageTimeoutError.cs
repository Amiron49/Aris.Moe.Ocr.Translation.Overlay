namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class ImageTimeoutError : CorrelatedError
    {
        public ImageTimeoutError() : base("Failed to load the image due to a timeout. Are you sure that the image is publicly reachable?")
        {
        }
    }
}