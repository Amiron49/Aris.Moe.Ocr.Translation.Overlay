using System;
using System.Collections.Generic;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class ImageReferenceModel
    {
        public Guid Id { get; init; }
        public ImageInfoModel Info { get; init; } = null!;
        public IEnumerable<ImageUrl> Urls { get; set; } = new List<ImageUrl>();
        public double QualityScore { get; init; }
    }

    public class ImageUrl
    {
        public Guid ImageReferenceId { get; init; }
        public byte[] UrlHash { get; init; } = null!;
        public string? OriginalUrl { get; init; }
    }
}