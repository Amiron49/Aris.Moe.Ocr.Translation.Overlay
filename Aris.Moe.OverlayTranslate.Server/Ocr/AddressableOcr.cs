using System;
using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public class AddressableOcr<TSpatial> where TSpatial: AddressableSpatialText
    {
        public Guid? ForImage { get; init; }
        public string Language { get; }
        public IEnumerable<TSpatial> Texts { get; }
        
        public AddressableOcr(string language, IEnumerable<TSpatial> texts)
        {
            Language = language;
            Texts = texts;
        }
    }        
}