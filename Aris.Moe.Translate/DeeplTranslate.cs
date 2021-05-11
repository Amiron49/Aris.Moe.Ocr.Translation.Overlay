using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Configuration;
using DeepL;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Translate
{
    public class DeeplTranslate : ITranslate, INeedConfiguration
    {
        private readonly IDeeplConfiguration _deeplConfiguration;
        private readonly ILogger<DeeplTranslate> _logger;
        private readonly Lazy<DeepLClient> _client;

        public DeeplTranslate(IDeeplConfiguration deeplConfiguration, ILogger<DeeplTranslate> logger)
        {
            _deeplConfiguration = deeplConfiguration;
            _logger = logger;
            _client = new Lazy<DeepLClient>(() => new DeepLClient(deeplConfiguration.ApiKey));
        }

        public Task<IEnumerable<Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            if (inputLanguage == "und")
                inputLanguage = null;
            
            return TranslateInternal(originals.ToList(), targetLanguage, inputLanguage);
        }

        private async Task<IEnumerable<Translation>> TranslateInternal(IList<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var translated = (await _client.Value.TranslateAsync(originals, inputLanguage?.ToUpperInvariant(), targetLanguage?.ToUpperInvariant())).ToList();

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

        public string Name { get; } = "DeeplTranslate";
        public IEnumerable<string> GetConfigurationIssues()
        {
            if (string.IsNullOrWhiteSpace(_deeplConfiguration.ApiKey))
                yield return "Missing api Key";
        }
    }
}