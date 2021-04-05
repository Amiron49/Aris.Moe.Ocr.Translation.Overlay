﻿using System;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageReference
    {
        public Guid Id { get; init; }
        public ImageInfo Info { get; init; }
        public string? OriginalUrl { get; init; }
        public double QualityScore { get; init; }

        public ImageReference(Guid id, ImageInfo info, string? originalUrl, double qualityScore)
        {
            Id = id;
            Info = info;
            OriginalUrl = originalUrl;
            QualityScore = qualityScore;
        }
    }
}