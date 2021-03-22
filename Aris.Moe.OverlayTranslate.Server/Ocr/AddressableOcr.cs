using System;
using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public class AddressableOcr<TSpatial> where TSpatial: AddressableSpatialText
    {
        public Guid? ForImage { get; set; }
        public string Language { get; init; }
        public virtual IEnumerable<TSpatial> Texts { get; init; }
    }
}