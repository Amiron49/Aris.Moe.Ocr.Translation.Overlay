namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class ExternalServerError : FluentResults.Error
    {
        public ExternalServerError() : base("Failed to load the image due to an error on the target server that hosts the image. Maybe try again later?")
        {
        }
    }
}