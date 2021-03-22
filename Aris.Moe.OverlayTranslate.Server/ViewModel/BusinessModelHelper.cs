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
            return new OcrViewModel
            {
                Texts = ocr.Texts.Select(x => ToViewModel(x, ocr.Language)),
                DetectedLanguage = ocr.Language
            };
        }
        
        public static TranslationViewModel ToTranslationViewModel(this Translation<BasedOnSpatialText> translation)
        {
            return new TranslationViewModel
            {
                Texts = translation.Texts.Select(x => x.ToViewModel(translation.Language)),
                Language = translation.Language
            };
        }

        public static SpatialTextViewModel ToViewModel(this ISpatialText text, string language)
        {
            return new()
            {
                Position = text.Area.ToModel(),
                Text = text.Text
            };
        }

        public static TranslationViewModel ToViewModel(this MachineTranslation translation)
        {
            return new()
            {
                Texts = translation.Texts.Select(x => x.ToViewModel(translation.Language)),
                Language = translation.Language
            };
        }
    }
}