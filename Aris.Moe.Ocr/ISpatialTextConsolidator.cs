using System.Collections.Generic;

namespace Aris.Moe.Ocr
{
    /// <summary>
    /// Consolidates Spatial Text to cohesive bigger texts
    /// </summary>
    public interface ISpatialTextConsolidator
    {
        IEnumerable<ISpatialText> Consolidate(IEnumerable<ISpatialText> texts);
    }
}