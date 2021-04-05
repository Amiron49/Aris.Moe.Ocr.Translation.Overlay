using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public class OcrViewModel
    {
        public string DetectedLanguage { get; init; }
        public IEnumerable<SpatialTextViewModel> Texts { get; init; }
        
        public OcrViewModel(string detectedLanguage, IEnumerable<SpatialTextViewModel> texts)
        {
            DetectedLanguage = detectedLanguage;
            Texts = texts;
        }
    }
}