﻿using System;
using System.Threading.Tasks;
using Aris.Moe.Ocr;
using Aris.Moe.Overlay;
using Aris.Moe.OverlayTranslate.Gui.Overlay.Modes;
using Aris.Moe.ScreenHelpers;
using Rectangle = System.Drawing.Rectangle;

namespace Aris.Moe.OverlayTranslate.Gui.Overlay
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
        private readonly IProgressDisplayGuiMode _progressDisplay;

        private OverlayMode _currentMode = OverlayMode.TextOverlay;

        public Overlay(IScreenInformation screenInformation, IProgressDisplayGuiMode progressDisplay) : base(screenInformation)
        {
            _textOverlay = new TextOverlay();
            _resizeOverlay = new TargetAreaResizeOverlay(screenInformation);
            _progressDisplay = progressDisplay;

            _progressDisplay.OnWantsToRender += (sender, args) =>
            {
                ShowOverlay();
            };
        }

        public async Task Init()
        {
            await Start();
        }

        protected override void Render()
        {
            if (_progressDisplay.ShouldRender)
                _progressDisplay.Render();

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

        // public void DisplayProgress(string description, CancellationTokenSource cancellationTokenSource, ProgressStep step, params ProgressStep[] moreSteps)
        // {
        //     ShowOverlay();
        //
        //     _progressOverlay.DisplayProgress(description, cancellationTokenSource, step, moreSteps);
        // }

        public void AskForResize(Rectangle current, Action<Rectangle?> resultCallback)
        {
            _currentMode = OverlayMode.ResizeTargetOverlay;

            SetClickAbility(true);
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