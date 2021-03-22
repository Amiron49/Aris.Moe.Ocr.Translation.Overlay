namespace Aris.Moe.OverlayTranslate.Server.Error
{
    public class CouldntReachImageError : FluentResults.Error
    {
        public CouldntReachImageError() : base(
            $"Image couldn't be reached. It may indicate that the target image may not be public or the (Honyaku-chan) server IP has been banned")
        {
        }
    }
}