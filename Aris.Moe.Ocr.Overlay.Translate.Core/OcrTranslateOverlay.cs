using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public class OcrTranslateOverlay : IOcrTranslateOverlay
    {
        private readonly ITranslate _translate;
        private readonly IOverlay _textOverlay;
        private readonly IScreenImageProvider _screenImageProvider;
        private readonly IOcr _ocr;
        private readonly IOcrTranslateOverlayConfiguration _ocrTranslateOverlayConfiguration;
        private readonly Action<string> _log;


        public OcrTranslateOverlay(ITranslate translate, IOverlay textOverlay, IScreenImageProvider screenImageProvider, IOcr ocr,
            IOcrTranslateOverlayConfiguration ocrTranslateOverlayConfiguration, Action<string> log)
        {
            _translate = translate;
            _textOverlay = textOverlay;
            _screenImageProvider = screenImageProvider;
            _ocr = ocr;
            _ocrTranslateOverlayConfiguration = ocrTranslateOverlayConfiguration;
            _log = log;

            if (_ocrTranslateOverlayConfiguration.ScreenArea == null)
                throw new ArgumentNullException(nameof(_ocrTranslateOverlayConfiguration.ScreenArea), "Screen capture area was not configured");
        }

        public async Task TranslateScreen()
        {
            ShowOverlay();

            var captureLocation = _ocrTranslateOverlayConfiguration.ScreenArea;
            
            _textOverlay.Add(new SpatialText("Will capture here", captureLocation));

            await Task.Delay(TimeSpan.FromSeconds(1));

            HideOverlay();
            _textOverlay.ClearAll();
            
            await Task.Delay(TimeSpan.FromSeconds(0.2));

            var screenImage = _screenImageProvider.Get(captureLocation);
            
            ShowOverlay();
            
            //TODO display captured image
            //_textOverlay.Add(screenImage.Original, new Rectangle(0, 0, screenImage.Original.Width/1, screenImage.Original.Height/1));

            var recognizedTextboxes = (await _ocr.Ocr(screenImage.Stream, _ocrTranslateOverlayConfiguration.SourceLanguage)).ToList();

            var textBoxesCount = recognizedTextboxes.Count();
            
            foreach (var spatialText in recognizedTextboxes)
                _textOverlay.Add(spatialText);
            
            if (textBoxesCount >= 60)
            {
                _log($"More text boxes than expected: {textBoxesCount}, aborting translation");
                return;
            }

            var translations = await _translate.Translate(recognizedTextboxes, _ocrTranslateOverlayConfiguration.TargetLanguage, _ocrTranslateOverlayConfiguration.SourceLanguage);

            var asSpatialText = translations.Select(x => new SpatialText(x.Text, x.Area));

            _textOverlay.ClearAll();
            
            foreach (var spatialText in asSpatialText)
                _textOverlay.Add(spatialText);

            ShowOverlay();
        }
        
        public async Task OcrScreen()
        {
            var recognizedTextboxes = await OcrScreenInternal();

            foreach (var spatialText in recognizedTextboxes)
                _textOverlay.Add(spatialText);

            ShowOverlay();
        }
        
        private async Task<IList<ISpatialText>> OcrScreenInternal()
        {
            ShowOverlay();

            var captureLocation = _ocrTranslateOverlayConfiguration.ScreenArea;
            
            _textOverlay.Add(new SpatialText("Will capture here", captureLocation));

            await Task.Delay(TimeSpan.FromSeconds(1));
            
            HideOverlay();
            
            _textOverlay.ClearAll();

            await Task.Delay(TimeSpan.FromSeconds(0.2));

            var screenImage = _screenImageProvider.Get(captureLocation);
            
            var recognizedTextboxes = (await _ocr.Ocr(screenImage.Stream, _ocrTranslateOverlayConfiguration.SourceLanguage)).ToList();

            return recognizedTextboxes;
        }

        public void HideOverlay()
        {
            _textOverlay.HideOverlay();
        }

        public void ToggleOverlay()
        {
            _textOverlay.ToggleOverlay();
        }

        public void ShowOverlay()
        {
            _textOverlay.ShowOverlay();
        }

        public void Dispose()
        {
            _textOverlay?.Dispose();
        }
    }
}