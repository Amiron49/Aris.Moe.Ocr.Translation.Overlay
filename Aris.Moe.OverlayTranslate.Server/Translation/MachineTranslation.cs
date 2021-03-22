using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public class MachineTranslation : Translation<BasedOnSpatialText>
    {
        public MachineTranslationProvider Provider { get; init; }
    }
}