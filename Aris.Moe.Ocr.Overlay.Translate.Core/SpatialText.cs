using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface ISpatialText
    {
        string Text { get; }
        Rectangle Area { get; }
        SpatialTextMetrics Metrics { get; }
        ISpatialText Combine(ISpatialText spatialText);
    }

    [DebuggerDisplay("{Text} FS:{Metrics.AverageLetterHeight}")]
    public class SpatialText : ISpatialText
    {
        private Lazy<SpatialTextMetrics> _metrics;
        public string Text { get; protected set; }
        public Rectangle Area { get; protected set; }

        public SpatialTextMetrics Metrics => _metrics.Value;

        public virtual ISpatialText Combine(ISpatialText spatialText)
        {
            return new AccumulatedSpatialText(this).Combine(spatialText);
        }

        public SpatialText(string text, Rectangle area)
        {
            Text = text;
            Area = area;

            _metrics = new Lazy<SpatialTextMetrics>(() => CreateMetrics(this));
        }

        protected virtual SpatialTextMetrics CreateMetrics(SpatialText text)
        {
            var letterCount = text.Text.Length;
            var averageLetterHeight = text.Area.Size.Height / letterCount;
            var averageLetterWidth = text.Area.Size.Width / letterCount;

            if (averageLetterHeight == 0)
                averageLetterHeight = 1;

            if (averageLetterWidth == 0)
                averageLetterWidth = 1;

            return new SpatialTextMetrics
            {
                AverageLetterHeight = averageLetterHeight,
                AverageLetterWidth = averageLetterWidth
            };
        }
    }

    [DebuggerDisplay("{Text} FS:{Metrics.AverageLetterHeight}")]
    public class AccumulatedSpatialText : SpatialText
    {
        private readonly IList<ISpatialText> _spatialTexts = new List<ISpatialText>();

        public AccumulatedSpatialText(ISpatialText a) : base(a.Text, a.Area)
        {
            _spatialTexts.Add(a);
        }

        public override ISpatialText Combine(ISpatialText spatialText)
        {
            _spatialTexts.Add(spatialText);

            Area = Rectangle.Union(Area, spatialText.Area);
            Text += spatialText.Text;

            return this;
        }

        protected override SpatialTextMetrics CreateMetrics(SpatialText text)
        {
            return _spatialTexts[0].Metrics;
        }
    }

    public class SpatialTextMetrics
    {
        public int AverageLetterHeight { get; set; }
        public int AverageLetterWidth { get; set; }
    }
}