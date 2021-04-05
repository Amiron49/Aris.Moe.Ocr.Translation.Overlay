namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class ImageTooSmallError : FluentResults.Error
    {
        public ImageTooSmallError(long size, long min) : base(
            $"Content length of the image response is suspiciously small, the minimum allowed size is '{min}' but the header returned by the target server indicated a size of '{size}'. Also did you circumvent the client side checks for the size? If not please remind me to put one in :v")
        {
        }
    }
}