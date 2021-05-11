using System.Drawing;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Ocr.Community
{
    public class CommunitySpatialText : BasedOnSpatialText
    {
        public int? UserId { get; init; }
        public int Score { get; init; }
        
        public CommunitySpatialText(string text, Rectangle area) : base(text, area)
        {
        }
    }
}