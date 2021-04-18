using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;

namespace Aris.Moe.OverlayTranslate.Server.Image.Error
{
    public class NoImageVerificationCharacteristicsError : CorrelatedError
    {
        public NoImageVerificationCharacteristicsError()
        {
            Message = "Need either a sha256 hash or height and width of the image for verification purposes";
        }
    }
}