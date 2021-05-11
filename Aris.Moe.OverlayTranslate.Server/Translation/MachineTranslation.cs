using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public class MachineTranslation : Translation<BasedOnSpatialText>
    {
        public MachineTranslationProvider Provider { get; init; }

        public MachineTranslation(string language, IEnumerable<BasedOnSpatialText> texts, MachineTranslationProvider provider) : base(language, texts)
        {
            Provider = provider;
        }
    }
}