using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr;
using Aris.Moe.ScreenHelpers;
using Aris.Moe.Translate;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Gui
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
            var recognizedTextboxes = (await OcrScreenInternal()).ToList();

            var textBoxesCount = recognizedTextboxes.Count;
            _log.LogInformation("Found {Count} textboxes", textBoxesCount);
            
            _internalOverlay.Add(recognizedTextboxes.ToArray());

            if (textBoxesCount >= 60)
            {
                _log.LogWarning("More text boxes than expected: {textBoxesCount}, aborting translation. Probably means that the OCR implementation is bonkers", textBoxesCount);
                return;
            }

            _log.LogInformation("Translating the textboxes");
            var originalTexts = recognizedTextboxes.Select(x => x.Text);
            var translations = (await _translate.Translate(originalTexts, _ocrTranslateOverlayConfiguration.TargetLanguage, _ocrTranslateOverlayConfiguration.SourceLanguage)).ToList();

            var asSpatialText = ToSpatialTexts(recognizedTextboxes, translations);

            var correctedForCaptureArea = asSpatialText.Select(CorrectForCaptureArea).ToList();
            
            _internalOverlay.ClearAll();

            _log.LogInformation("Passing translated texts to overlay");
            _internalOverlay.Add(correctedForCaptureArea.ToArray());

            ShowOverlay();
            _log.LogInformation("Showing the complete translation");
        }

        private static IEnumerable<SpatialText> ToSpatialTexts(IList<ISpatialText> originals, IList<Translation> translations)
        {
            for (var i = 0; i < originals.Count; i++)
            {
                var original = originals[i];
                var translation = translations[i];

                yield return new SpatialText(translation.Text, original.Area);
            }
        }

        public async Task OcrScreen()
        {
            var recognizedTextboxes = await OcrScreenInternal();

            var correctedForCaptureArea = recognizedTextboxes.Select(CorrectForCaptureArea).ToList();
            
            _internalOverlay.ClearAll();
            
            foreach (var spatialText in correctedForCaptureArea)
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
            
            var (stream, bitmap) = _screenImageProvider.Get(captureLocation);

            using (stream)
            {
                using (bitmap)
                {
                    if (_ocrTranslateOverlayConfiguration.DebugCapturedImage)
                        await DebugImage(stream, bitmap);
            
                    _log.LogInformation("Ocr-ing the image");
                    var recognizedTextboxes = await _ocr.Ocr(stream, _ocrTranslateOverlayConfiguration.SourceLanguage);

                    return recognizedTextboxes.Texts.ToList();
                }
            }
        }

        private async Task DebugImage(Stream stream, Image bitmap)
        {
            var currentPosition = stream.Position;

            using (var ms = new MemoryStream())
            {
                if (!Directory.Exists(_ocrTranslateOverlayConfiguration.CacheFolderRoot))
                    Directory.CreateDirectory(_ocrTranslateOverlayConfiguration.CacheFolderRoot);

                await stream.CopyToAsync(ms);
                File.WriteAllBytes(Path.Combine(_ocrTranslateOverlayConfiguration.CacheFolderRoot, "capture-stream.png"), ms.ToArray());
                bitmap.Save(Path.Combine(_ocrTranslateOverlayConfiguration.CacheFolderRoot, "capture-bmp.png"), ImageFormat.Png);
            }

            stream.Position = currentPosition;
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

        // private double progressCount = 0;
        // private System.Timers.Timer timer;
        
        public void DisplayProgress()
        {
            // progressCount = 0;
            // Progress<double> progress = new Progress<double>();
            //
            // timer = new System.Timers.Timer(1000);
            //
            // timer.Elapsed += (sender, args) =>
            // {
            //     progressCount += 0.1d;
            //     ((IProgress<double>)progress).Report(progressCount);
            //     if (progressCount >= 1) 
            //     {
            //         timer.Stop();
            //         timer.Dispose();
            //     }
            // };
            // timer.AutoReset = true;
            // timer.Enabled = true;
            // timer.Start();
            //
            // var cancellationTokenSource = new CancellationTokenSource();
            // _internalOverlay.DisplayProgress("lel", cancellationTokenSource, new ProgressStep("lel step", progress));
        }

        public void Dispose()
        {
        }
    }
}