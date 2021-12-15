using FluentResults;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class AlreadyInEnglishOCRError : Error
    {
        public AlreadyInEnglishOCRError() : base("OCR THINKS it's already in english :(")
        {
        }
    }
}