namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class ImageTooBigError : CorrelatedError
    {
        public ImageTooBigError(long size, long max) : base(
            $"Content length of the image response is suspiciously large, the maximum allowed size is '{max}' but the header returned by the target server indicated a size of '{size}'. Also did you circumvent the client side checks for the size? If not please remind me to put one in :v")
        {
        }
    }
}