﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public class OcrTranslateOverlay : IOcrTranslateOverlay
    {
        private readonly ITranslate _translate;
        private readonly IOverlay _internalOverlay;
        private readonly IScreenImageProvider _screenImageProvider;
        private readonly IOcr _ocr;
        private readonly IOcrTranslateOverlayConfiguration _ocrTranslateOverlayConfiguration;
        private readonly ILogger<OcrTranslateOverlay> _log;


        public OcrTranslateOverlay(ITranslate translate, IOverlay internalOverlay, IScreenImageProvider screenImageProvider, IOcr ocr,
            IOcrTranslateOverlayConfiguration ocrTranslateOverlayConfiguration, ILogger<OcrTranslateOverlay> log)
        {
            _translate = translate;
            _internalOverlay = internalOverlay;
            _screenImageProvider = screenImageProvider;
            _ocr = ocr;
            _ocrTranslateOverlayConfiguration = ocrTranslateOverlayConfiguration;
            _log = log;

            if (_ocrTranslateOverlayConfiguration.CaptureArea == null)
                throw new ArgumentNullException(nameof(_ocrTranslateOverlayConfiguration.CaptureArea), "Screen capture area was not configured");
        }

        public async Task TranslateScreen()
        {
            var recognizedTextboxes = await OcrScreenInternal();

            var textBoxesCount = recognizedTextboxes.Count;
            _log.LogInformation("Found {Count} textboxes", textBoxesCount);
            
            _internalOverlay.Add(recognizedTextboxes.ToArray());

            if (textBoxesCount >= 60)
            {
                _log.LogWarning("More text boxes than expected: {textBoxesCount}, aborting translation. Probably means that the OCR implementation is bonkers", textBoxesCount);
                return;
            }

            _log.LogInformation("Translating the textboxes");
            var translations = await _translate.Translate(recognizedTextboxes, _ocrTranslateOverlayConfiguration.TargetLanguage, _ocrTranslateOverlayConfiguration.SourceLanguage);

            var asSpatialText = translations.Select(x => new SpatialText(x.Text, x.Area));

            var correctedForCaptureArea = asSpatialText.Select(CorrectForCaptureArea).ToList();
            
            _internalOverlay.ClearAll();

            _log.LogInformation("Passing translated texts to overlay");
            _internalOverlay.Add(correctedForCaptureArea.ToArray());

            ShowOverlay();
            _log.LogInformation("Showing the complete translation");
        }

        public async Task OcrScreen()
        {
            var recognizedTextboxes = await OcrScreenInternal();

            foreach (var spatialText in recognizedTextboxes)
                _internalOverlay.Add(spatialText);

            ShowOverlay();
        }

        private async Task<IList<ISpatialText>> OcrScreenInternal()
        {
            ShowOverlay();

            var captureLocation = _ocrTranslateOverlayConfiguration.CaptureArea;

            _internalOverlay.ClearAll();
            
            _internalOverlay.Add(new SpatialText("Will capture here", captureLocation));

            await Task.Delay(TimeSpan.FromSeconds(1));

            _log.LogInformation("Hiding overlay for ocr");
            HideOverlay();

            await Task.Delay(TimeSpan.FromSeconds(0.2));

            _log.LogInformation("Capturing screen");
            var screenImage = _screenImageProvider.Get(captureLocation);

            _log.LogInformation("Ocr-ing the image");
            var recognizedTextboxes = await _ocr.Ocr(screenImage.Stream, _ocrTranslateOverlayConfiguration.SourceLanguage);

            return recognizedTextboxes.ToList();
        }

        private ISpatialText CorrectForCaptureArea(ISpatialText spatialText)
        {
            var captureAreaLocation = _ocrTranslateOverlayConfiguration.CaptureArea.Location;
            spatialText.Move(captureAreaLocation);
            return spatialText;
        }

        public void HideOverlay()
        {
            _internalOverlay.HideOverlay();
        }

        public void ToggleOverlay()
        {
            _internalOverlay.ToggleOverlay();
        }

        public void ShowOverlay()
        {
            _internalOverlay.ShowOverlay();
        }

        public void AskForTargetResize()
        {
            _internalOverlay.AskForResize(_ocrTranslateOverlayConfiguration.CaptureArea, rectangle =>
            {
                if (rectangle == null)
                    return;

                _ocrTranslateOverlayConfiguration.CaptureArea = rectangle.Value;
            });
        }

        public void Dispose()
        {
            _internalOverlay.Dispose();
        }
    }
}