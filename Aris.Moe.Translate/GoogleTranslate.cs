﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Configuration;
using Aris.Moe.Ocr;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Translate
{
    public class GoogleTranslate : ITranslate, INeedConfiguration
    {
        private readonly IGoogleConfiguration _googleConfiguration;
        private readonly ILogger<GoogleTranslate> _log;
        private readonly Lazy<TranslationServiceClient> _translateClientLazy;

        public GoogleTranslate(IGoogleConfiguration googleConfiguration, ILogger<GoogleTranslate> logger)
        {
            _googleConfiguration = googleConfiguration;
            _log = logger;

            _translateClientLazy = new Lazy<TranslationServiceClient>(() => new TranslationServiceClientBuilder
            {
                CredentialsPath = googleConfiguration.KeyPath
            }.Build());
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

        private async Task<RepeatedField<Google.Cloud.Translate.V3.Translation>> TranslateWithGoogle(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage,
            string? inputLanguage)
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

            var response = await _translateClientLazy.Value.TranslateTextAsync(request);

            return response.Translations;
        }

        public string Name { get; } = "Google-Translate-V3";

        public IEnumerable<string> GetConfigurationIssues()
        {
            if (string.IsNullOrEmpty(_googleConfiguration.KeyPath))
                yield return $"'{nameof(IGoogleConfiguration.KeyPath)}': is not set. A private key enabled to access the V3Translation Api is needed";
            else if (!File.Exists(_googleConfiguration.KeyPath))
                yield return $"Couldn't find google key file @ {_googleConfiguration.KeyPath}. A private key enabled to access the V3Translation Api is needed";

            if (string.IsNullOrEmpty(_googleConfiguration.LocationId))
                yield return
                    $"'{nameof(IGoogleConfiguration.ProjectId)}': is not set. Your own google cloud projectId must be given for it to work. Also the TranslateV3 Api must be enabled for that project";
        }
    }
}