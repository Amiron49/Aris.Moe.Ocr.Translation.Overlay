using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public class TranslationViewModel
    {
        public string Language { get; init; }
        public IEnumerable<SpatialTextViewModel> Texts { get; init; }
    }
}