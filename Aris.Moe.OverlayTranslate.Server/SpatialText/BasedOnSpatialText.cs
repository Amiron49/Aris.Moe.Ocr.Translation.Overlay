using System.Drawing;

namespace Aris.Moe.OverlayTranslate.Server.SpatialText
{
    public class BasedOnSpatialText : AddressableSpatialText
    {
        public int? BasedOn { get; init; }
        public BasedOnSpatialText(string text, Rectangle area) : base(text, area)
        {
        }
    }
}