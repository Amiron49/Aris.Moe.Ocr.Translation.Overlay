using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public class Translation<TSpatial> where TSpatial : BasedOnSpatialText
    {
        public string Language { get; init; }
        public IEnumerable<TSpatial> Texts { get; init; }
    }
}