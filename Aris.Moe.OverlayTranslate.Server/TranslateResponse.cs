using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class TranslateResponse
    {
        public MatchType Match { get; }
        public ImageInfo Image { get; }
        public IEnumerable<TranslationViewModel> MachineTranslations { get; }
        
        public TranslateResponse(MatchType match, ImageInfo image, IEnumerable<TranslationViewModel> machineTranslations)
        {
            Match = match;
            Image = image;
            MachineTranslations = machineTranslations;
        }
    }
}