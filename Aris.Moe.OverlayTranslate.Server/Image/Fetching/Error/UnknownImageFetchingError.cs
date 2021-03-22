namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Errors
{
    public class UnknownImageFetchingError : FluentResults.Error
    {
        public UnknownImageFetchingError(string errorCorrelation) : base(
            $"Something not anticipated by me went wrong when fetching the image. Very likely it's my fault (or you are being naughty and triggered a safety feature). Please make a bug report with this correlationID: {errorCorrelation}")
        {
        }
    }
}