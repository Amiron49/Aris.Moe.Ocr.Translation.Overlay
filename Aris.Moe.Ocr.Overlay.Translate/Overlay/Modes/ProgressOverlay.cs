using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Aris.Moe.Core;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.ScreenHelpers;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes
{
    public interface IProgressDisplayGuiMode : IProgressDisplay, IGuiMode
    {
    }

    public class ProgressDisplay : IProgressDisplayGuiMode
    {
        private readonly ILogger<ProgressDisplay> _logger;

        private const ImGuiWindowFlags NoDecoration = ImGuiWindowFlags.NoDecoration |
                                                      ImGuiWindowFlags.NoMove |
                                                      ImGuiWindowFlags.NoNav |
                                                      ImGuiWindowFlags.NoScrollbar |
                                                      ImGuiWindowFlags.NoTitleBar |
                                                      ImGuiWindowFlags.NoResize |
                                                      ImGuiWindowFlags.NoInputs;


        private const int TotalWidth = 300;
        private const int TotalHeight = 200;
        private const int ProgressbarWidth = (int) (TotalWidth - Margin * 2);
        private const float ProgressbarHeight = 10f;
        private const float Margin = 16f;
        private readonly int _xOffset;
        private readonly int _yOffset;

        public ProgressDisplay(IScreenInformation screenInformation, ILogger<ProgressDisplay> logger)
        {
            _logger = logger;
            _xOffset = 0;
            _yOffset = screenInformation.ScreenArea.Height - TotalHeight;
        }

        public void DisplayProgress(string description, CancellationTokenSource cancellationTokenSource, ProgressStep step, params ProgressStep[] moreSteps)
        {
            var progressOperation = new ProgressOperation(description, cancellationTokenSource, new[] {step}.Concat(moreSteps));

            progressOperation.OnFinished += (sender, args) => { _logger.LogInformation("Done with step"); };

            _progressOperationQueue.Enqueue(progressOperation);
            OnWantsToRender?.Invoke(this, EventArgs.Empty);
        }

        private volatile ConcurrentQueue<ProgressOperation> _progressOperationQueue = new ConcurrentQueue<ProgressOperation>();
        private ProgressOperation? _currentProgressOperation;


        public bool ShouldRender => (_currentProgressOperation != null && !_currentProgressOperation.Done && !_currentProgressOperation.Cancelled) || _progressOperationQueue.Any();
        public event EventHandler? OnWantsToRender;

        public void Render()
        {
            if (_currentProgressOperation == null || _currentProgressOperation.Done || _currentProgressOperation.Cancelled)
            {
                if (!_progressOperationQueue.Any())
                    return;

                _progressOperationQueue.TryDequeue(out _currentProgressOperation);
            }

            var pair = _currentProgressOperation.Current;

            if (pair == null)
                return;

            var currentStep = pair.Value.Step;
            var index = pair.Value.Index;

            if (currentStep == null)
                return;

            RenderBackdrop(() =>
            {
                ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.2f, 0.4f, 1.0f, 1.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.8f);

                ImGui.Text(currentStep.StepDescription);
                ImGui.Text($"{index}/{_currentProgressOperation.Steps.Count}");
                var currentStepProgressPercentage = currentStep.ProgressPercentage * 100;
                ImGui.Text($"{currentStepProgressPercentage:F}%");

                ProgressBar(currentStep);

                ImGui.End();
                ImGui.PopStyleColor();
                ImGui.PopStyleVar(0);
            });
        }

        private void RenderBackdrop(Action inner)
        {
            ImGui.SetNextWindowSize(new Vector2(TotalWidth, TotalHeight));

            ImGui.SetNextWindowPos(new Vector2(_xOffset, _yOffset));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);

            ImGui.Begin("ProgressBackground", NoDecoration);
            ImGui.PopStyleVar();

            inner();
            ImGui.End();
        }

        private void ProgressBar(ProgressStep currentStep)
        {
            var yProgressBar = _yOffset + 32f * 4;
            var xProgressBar = _xOffset + Margin;

            var upperLeft = new Vector2(xProgressBar, yProgressBar);
            var lowerRightY = yProgressBar + ProgressbarHeight;

            var lowerRightOutline = new Vector2(xProgressBar + ProgressbarWidth, lowerRightY);
            var lowerRightFilled = new Vector2((float) (xProgressBar + ProgressbarWidth * currentStep.ProgressPercentage), lowerRightY);

            ImGui.GetWindowDrawList().AddRect(upperLeft, lowerRightOutline, ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.4f, 1.0f, 1.0f)));
            ImGui.GetWindowDrawList().AddRectFilled(upperLeft, lowerRightFilled, ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.2f, 1.0f, 1.0f)));
        }
    }

    public class ProgressOperation
    {
        public string ProgressDescription { get; }
        public CancellationTokenSource CancellationToken { get; set; }
        public bool Cancelled => CancellationToken.IsCancellationRequested;
        public bool Done => Steps.All(x => x.Step.ProgressPercentage >= 1);
        public IReadOnlyList<(ProgressStep Step, int Index)> Steps { get; }

        public (ProgressStep Step, int Index)? Current => Steps.FirstOrDefault(x => x.Step.ProgressPercentage < 1);

        public event EventHandler? OnFinished;

        public ProgressOperation(string progressDescription, CancellationTokenSource cancellationToken, IEnumerable<ProgressStep> steps)
        {
            ProgressDescription = progressDescription;
            CancellationToken = cancellationToken;
            Steps = steps.Select((x, i) =>
            {
                x.OnFinished += (sender, args) => CheckForCompletion();
                return (x, i);
            }).ToList();
        }

        private void CheckForCompletion()
        {
            if (Done)
                OnFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}