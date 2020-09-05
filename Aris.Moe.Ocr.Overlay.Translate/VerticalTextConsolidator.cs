using System;
using System.Collections.Generic;
using System.Linq;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class VerticalTextConsolidator : ISpatialTextConsolidator
    {
        public IEnumerable<ISpatialText> Consolidate(IEnumerable<ISpatialText> texts)
        {
            var asList = texts.ToList();
            
            if (!asList.Any())
                return new ISpatialText[0];

            return ConsolidateInternal(asList);
        }

        private IEnumerable<ISpatialText> ConsolidateInternal(IEnumerable<ISpatialText> unconsolidatedTexts)
        {
            var consolidatedTexts = new List<ISpatialText>();

            foreach (var text in unconsolidatedTexts)
            {
                var consolidatedTextItBelongsTo = consolidatedTexts.FirstOrDefault(x => BelongToSameText(x, text));

                if (consolidatedTextItBelongsTo == null)
                    consolidatedTexts.Add(new AccumulatedSpatialText(text));
                else
                    consolidatedTextItBelongsTo.Combine(text);
            }
            
            return consolidatedTexts;
        }
        
        private static bool BelongToSameText(ISpatialText textA, ISpatialText textB)
        {
            var maxDistanceA = textA.Metrics.AverageLetterWidth + textA.Metrics.AverageLetterHeight;
            var maxDistanceB = textB.Metrics.AverageLetterWidth + textB.Metrics.AverageLetterHeight;

            var areCloseEnoughForA = textA.Area.IsInDistance(textB.Area, maxDistanceA);
            var areCloseEnoughForB = textA.Area.IsInDistance(textB.Area, maxDistanceB);

            return areCloseEnoughForA && areCloseEnoughForB;
        }
    }
}