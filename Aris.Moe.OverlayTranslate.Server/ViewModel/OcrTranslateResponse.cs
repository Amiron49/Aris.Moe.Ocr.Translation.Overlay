using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.Image;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public class OcrTranslateResponse : TranslateResponse
    {
        public IEnumerable<OcrViewModel> MachineOcrs { get; }

        public OcrTranslateResponse(MatchType match, ImageInfo image, IEnumerable<TranslationViewModel> machineTranslations, IEnumerable<OcrViewModel> machineOcrs) : base(match, image, machineTranslations)
        {
            MachineOcrs = machineOcrs;
        }
    }
}