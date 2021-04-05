using System.Linq;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Server.Ocr;
using Aris.Moe.OverlayTranslate.Server.SpatialText;
using Aris.Moe.OverlayTranslate.Server.Translation;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public static class BusinessModelHelper
    {
        public static OcrViewModel ToOcrViewModel(this AddressableOcr<AddressableSpatialText> ocr)
        {
            return new(
                ocr.Language,
                ocr.Texts.Select(x => ToViewModel(x, ocr.Language))
            );
        }

        public static TranslationViewModel ToTranslationViewModel(this Translation<BasedOnSpatialText> translation)
        {
            return new(
                translation.Language,
                translation.Texts.Select(x => x.ToViewModel(translation.Language))
            );
        }

        public static SpatialTextViewModel ToViewModel(this ISpatialText text, string language)
        {
            return new(
                text.Text,
                text.Area.ToModel()
            );
        }

        public static TranslationViewModel ToViewModel(this MachineTranslation translation)
        {
            return new(
                translation.Language,
                translation.Texts.Select(x => x.ToViewModel(translation.Language))
            );
        }
    }
}