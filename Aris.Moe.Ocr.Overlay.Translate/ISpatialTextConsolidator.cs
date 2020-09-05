using System.Collections.Generic;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    /// <summary>
    /// Consolidates Spatial Text to cohesive bigger texts
    /// </summary>
    public interface ISpatialTextConsolidator
    {
        IEnumerable<ISpatialText> Consolidate(IEnumerable<ISpatialText> texts);
    }
}