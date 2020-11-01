using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Google.Cloud.Vision.V1;
using Image = Google.Cloud.Vision.V1.Image;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class GoogleOcr : IOcr, INeedConfiguration
    {
        private readonly IGoogleConfiguration _googleConfiguration;
        private readonly Lazy<ImageAnnotatorClient> _ocrClientLazy;

        public GoogleOcr(IGoogleConfiguration googleConfiguration)
        {
            _googleConfiguration = googleConfiguration;

            _ocrClientLazy = new Lazy<ImageAnnotatorClient>(() => new ImageAnnotatorClientBuilder
            {
                CredentialsPath = googleConfiguration.KeyPath
            }.Build());
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            var googleResult = await OcrFromGoogle(image, inputLanguage);

            var singleCharacterAnnotations = googleResult.Skip(1);

            var asSpatialTexts = singleCharacterAnnotations.Select(ConvertToSpatialText);

            return asSpatialTexts;
        }

        private async Task<IReadOnlyList<EntityAnnotation>> OcrFromGoogle(Stream image, string? inputLanguage = null)
        {
            image.Position = 0;
            var googleImage = await Image.FromStreamAsync(image);

            var imageContext = new ImageContext();

            if (inputLanguage != null)
                imageContext.LanguageHints.Add(inputLanguage);

            var response = await _ocrClientLazy.Value.DetectTextAsync(googleImage, imageContext);

            return response;
        }

        private static SpatialText ConvertToSpatialText(EntityAnnotation annotation)
        {
            var topLeft = annotation.BoundingPoly.Vertices[0];
            var bottomRight = annotation.BoundingPoly.Vertices[2];

            var position = new Point(topLeft.X, topLeft.Y);
            var size = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

            var rectangle = new Rectangle(position, size);

            return new SpatialText(annotation.Description, rectangle);
        }

        public string Name { get; } = "Google Ocr V3";
        public IEnumerable<string> GetConfigurationIssues()
        {
            if (string.IsNullOrEmpty(_googleConfiguration.KeyPath))
                yield return $"'{nameof(IGoogleConfiguration.KeyPath)}': is not set. A private key enabled to access the Google Ocr V3 Api is needed";
            else if (!File.Exists(_googleConfiguration.KeyPath))
                yield return $"Couldn't find google key file @ {_googleConfiguration.KeyPath}. A private key enabled to access the Google Ocr Api is needed";
        }
    }
}