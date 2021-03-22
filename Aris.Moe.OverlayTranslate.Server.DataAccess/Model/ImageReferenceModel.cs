using System;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class ImageReferenceModel
    {
        public Guid Id { get; init; }
        public ImageInfoModel Info { get; init; }
        public string? OriginalUrl { get; init; }
        public double QualityScore { get; init; }
    }
}