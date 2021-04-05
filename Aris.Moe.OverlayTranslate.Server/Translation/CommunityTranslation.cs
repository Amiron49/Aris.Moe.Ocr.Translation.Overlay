using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.Ocr.Community;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public class CommunityTranslation : Translation<CommunitySpatialText>
    {
        public CommunityTranslation(string language, IEnumerable<CommunitySpatialText> texts) : base(language, texts)
        {
        }
    }
}