using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using DeepL;
using Microsoft.Extensions.Logging;
using Translation = Aris.Moe.Ocr.Overlay.Translate.Core.Translation;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class DeeplTranslate : ITranslate
    {
        private readonly ILogger<DeeplTranslate> _logger;
        private readonly DeepLClient _client;

        public DeeplTranslate(IDeeplConfiguration deeplConfiguration, ILogger<DeeplTranslate> logger)
        {
            _logger = logger;
            _client = new DeepLClient(deeplConfiguration.ApiKey);
        }

        public Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            return TranslateInternal(spatialTexts.ToList(), targetLanguage, inputLanguage);
        }

        private async Task<IEnumerable<Translation>> TranslateInternal(IList<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var translated = (await _client.TranslateAsync(spatialTexts.Select(x => x.Text), inputLanguage?.ToUpperInvariant(), targetLanguage?.ToUpperInvariant())).ToList();

            if (translated.Count != spatialTexts.Count())
            {
                _logger.LogError($"ocr count ({spatialTexts.Count}) and translation count {translated.Count} mismatch");
                return new List<Translation>();
            }

            var result = new List<Translation>();

            for (var i = 0; i < translated.Count; i++)
            {
                var translation = translated[i];
                var spatial = spatialTexts[i];
                result.Add(new Translation(translation.Text, spatial.Text.Length, spatial.Area));
            }

            return result;
        }
    }
}