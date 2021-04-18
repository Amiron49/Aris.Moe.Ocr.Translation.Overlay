using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;

namespace Aris.Moe.OverlayTranslate.Server.Image.Error
{
    public class CouldntReachImageError : CorrelatedError
    {
        public CouldntReachImageError() : base(
            $"Image couldn't be reached. It may indicate that the target image may not be public or the (Honyaku-chan) server IP has been banned")
        {
        }
    }
}