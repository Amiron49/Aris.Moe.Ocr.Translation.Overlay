using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class TranslateResponse
    {
        public MatchType Match { get; init; }
        public ImageInfo? Image { get; init; }
        public IEnumerable<TranslationViewModel> MachineTranslations { get; init; }
    }
}