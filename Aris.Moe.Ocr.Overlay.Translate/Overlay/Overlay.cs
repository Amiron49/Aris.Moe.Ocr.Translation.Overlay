using System;
using System.Drawing;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes;
using Aris.Moe.Overlay;
using Microsoft.Extensions.Logging;
using Rectangle = System.Drawing.Rectangle;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay
{
    public class Overlay : ImGuiOverlay, IOverlay
    {
        private enum OverlayMode
        {
            TextOverlay,
            ResizeTargetOverlay
        }

        public bool Ready => ThreadIsReady;

        private readonly TextOverlay _textOverlay;

        private readonly TargetAreaResizeOverlay _resizeOverlay;

        private OverlayMode _currentMode = OverlayMode.TextOverlay;

        public Overlay(ILogger<TargetAreaResizeOverlay> logger)
        {
            _textOverlay = new TextOverlay();
            _resizeOverlay = new TargetAreaResizeOverlay(new Rectangle(0, 0, 1920, 1080), logger);
        }

        public async Task Init()
        {
            await Start();
        }

        protected override void Render()
        {
            switch (_currentMode)
            {
                case OverlayMode.TextOverlay:
                    _textOverlay.Render();
                    break;
                case OverlayMode.ResizeTargetOverlay:
                    _resizeOverlay.Render();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(params ISpatialText[] texts)
        {
            _textOverlay.Add(texts);
        }

        public void Add(Bitmap image, Rectangle targetArea)
        {
            //TODO I will never do :v
        }

        public void ClearAll()
        {
            _textOverlay.ClearAll();
        }

        public void HideOverlay()
        {
            SetVisibility(false);
            Visible = false;
        }

        public void ShowOverlay()
        {
            SetVisibility(true);
            Visible = true;
        }

        public void ToggleOverlay()
        {
            if (Visible)
                HideOverlay();
            else
                ShowOverlay();
        }

        public void AskForResize(Rectangle current, Action<Rectangle?> resultCallback)
        {
            _currentMode = OverlayMode.ResizeTargetOverlay;

            SetClickAbility(true);
            //BringToForeground();
            ShowOverlay();

            _resizeOverlay.AskForResize(current, resultRectangle =>
            {
                _currentMode = OverlayMode.TextOverlay;
                SetClickAbility(false);
                resultCallback(resultRectangle);
            });
        }
    }
}