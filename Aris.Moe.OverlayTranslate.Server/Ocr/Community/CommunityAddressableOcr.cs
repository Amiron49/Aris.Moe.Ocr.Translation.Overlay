using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Server.Ocr.Community
{
    public class CommunityAddressableOcr: AddressableOcr<CommunitySpatialText>
    {
        public CommunityAddressableOcr(string language, IEnumerable<CommunitySpatialText> texts) : base(language, texts)
        {
        }
    }
}