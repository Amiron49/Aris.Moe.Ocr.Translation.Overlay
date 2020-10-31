using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using Translation = Aris.Moe.Ocr.Overlay.Translate.Core.Translation;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class GoogleTranslate : ITranslate
    {
        private readonly IGoogleConfiguration _googleConfiguration;
        private readonly ILogger<GoogleTranslate> _log;
        private readonly TranslationServiceClient _translateClient;

        public GoogleTranslate(IGoogleConfiguration googleConfiguration, ILogger<GoogleTranslate> logger)
        {
            if (string.IsNullOrEmpty(googleConfiguration.KeyPath))
                throw new ArgumentNullException(nameof(googleConfiguration.KeyPath));

            _googleConfiguration = googleConfiguration;
            _log = logger;

            _translateClient = new TranslationServiceClientBuilder
            {
                CredentialsPath = googleConfiguration.KeyPath
            }.Build();
        }

        public Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            return TranslateInternal(spatialTexts.ToList(), targetLanguage, inputLanguage);
        }

        private async Task<IEnumerable<Translation>> TranslateInternal(IList<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var googleTranslations = await TranslateWithGoogle(spatialTexts, targetLanguage, inputLanguage);

            if (googleTranslations.Count != spatialTexts.Count())
            {
                _log.LogWarning($"ocr count ({spatialTexts.Count}) and translation count {googleTranslations.Count} mismatch");
                return new Translation[0];
            }

            var asTranslations = spatialTexts.Select((spatialText, i) =>
            {
                var googleTranslation = googleTranslations[i];

                return new Translation(googleTranslation.TranslatedText, spatialText.Text.Length, spatialText.Area);
            });

            return asTranslations;
        }

        private async Task<RepeatedField<Google.Cloud.Translate.V3.Translation>> TranslateWithGoogle(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage, string? inputLanguage)
        {
            var untranslatedTexts = spatialTexts.Select(x => x.Text).ToList();

            if (!untranslatedTexts.Any())
            {
                _log.LogWarning("No texts given for translation");
                return new RepeatedField<Google.Cloud.Translate.V3.Translation>();
            }

            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    untranslatedTexts
                },
                TargetLanguageCode = targetLanguage,
                SourceLanguageCode = inputLanguage,
                MimeType = "text/plain",
                ParentAsLocationName = new LocationName(_googleConfiguration.ProjectId, _googleConfiguration.LocationId)
            };

            var response = await _translateClient.TranslateTextAsync(request);

            return response.Translations;
        }
    }
}