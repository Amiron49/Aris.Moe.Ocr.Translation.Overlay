using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public class OcrTranslateResponse : TranslateResponse
    {
        public IEnumerable<OcrViewModel> MachineOcrs { get; init; }
    }
}