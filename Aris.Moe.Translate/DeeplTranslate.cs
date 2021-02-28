using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Configuration;
using DeepL;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Translate
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

        public Task<IEnumerable<Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            return TranslateInternal(originals.ToList(), targetLanguage, inputLanguage);
        }

        private async Task<IEnumerable<Translation>> TranslateInternal(IList<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var translated = (await _client.TranslateAsync(originals, inputLanguage?.ToUpperInvariant(), targetLanguage?.ToUpperInvariant())).ToList();

            if (translated.Count != originals.Count())
            {
                _logger.LogError($"ocr count ({originals.Count}) and translation count {translated.Count} mismatch");
                return new List<Translation>();
            }

            var result = new List<Translation>();

            for (var i = 0; i < translated.Count; i++)
            {
                var translation = translated[i];
                var original = originals[i];
                _logger.LogInformation(translation.Text);
                result.Add(new Translation(translation.Text, original));
            }

            return result;
        }
    }
}