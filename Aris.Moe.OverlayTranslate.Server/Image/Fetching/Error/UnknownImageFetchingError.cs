namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class UnknownImageFetchingError : CorrelatedError
    {
        public UnknownImageFetchingError() : base(
            "Something not anticipated by me went wrong when fetching the image. Very likely it's my fault (or you are being naughty and triggered a safety feature).")
        {
        }
    }
}