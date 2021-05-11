using System;
using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Server.SpatialText
{
    public class AddressableSpatialText : Moe.Ocr.SpatialText
    {
        public int? Id { get; init; }
        public DateTime Created { get; init; }
        
        public AddressableSpatialText(string text, Rectangle area) : base(text, area)
        {
        }
    }
}