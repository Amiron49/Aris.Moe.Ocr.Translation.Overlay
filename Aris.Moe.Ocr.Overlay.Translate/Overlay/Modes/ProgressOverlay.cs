using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Aris.Moe.ScreenHelpers;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr.Overlay.Translate.Overlay.Modes
{
    public class ProgressOverlay : IProgressOverlay, IGuiMode
    {
        private const ImGuiWindowFlags NoDecoration = ImGuiWindowFlags.NoDecoration |
                                                      ImGuiWindowFlags.NoMove |
                                                      ImGuiWindowFlags.NoNav |
                                                      ImGuiWindowFlags.NoScrollbar |
                                                      ImGuiWindowFlags.NoTitleBar |
                                                      ImGuiWindowFlags.NoResize |
                                                      ImGuiWindowFlags.NoInputs;

        private readonly IScreenInformation _screenInformation;

        public ProgressOverlay(IScreenInformation screenInformation)
        {
            _screenInformation = screenInformation;
        }

        public void DisplayProgress(string description, CancellationTokenSource cancellationTokenSource, ProgressStep step, params ProgressStep[] moreSteps)
        {
            //todo support queuing them
            var progressOperation = new ProgressOperation(description, cancellationTokenSource, new[] {step}.Concat(moreSteps));
            
            progressOperation.OnFinished += (sender, args) => _progressOperation.
            
            _progressOperation = progressOperation;
        }

        private volatile ConcurrentStack<ProgressOperation> _progressOperation = new ConcurrentStack<ProgressOperation>();

        public void Render()
        {
            if (_progressOperation == null || _progressOperation.Done || _progressOperation.Cancelled)
                return;

            var pair = _progressOperation.Current;

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
                ImGui.Text($"{index}/{_progressOperation.Steps.Count}");
                ImGui.GetWindowDrawList().AddRect(new Vector2(0, 0), new Vector2(30, 30), ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.4f, 1.0f, 1.0f)));
                ImGui.GetWindowDrawList().AddRectFilled(new Vector2(0, 0), new Vector2(15, 15), ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.2f, 1.0f, 1.0f)));

                ImGui.End();
                ImGui.PopStyleColor();
                ImGui.PopStyleVar(0);
            });
        }

        private void RenderBackdrop(Action inner)
        {
            ImGui.SetNextWindowSize(new Vector2(500, 300));
            ImGui.SetNextWindowPos(new Vector2(_screenInformation.ScreenArea.Width / 2, _screenInformation.ScreenArea.Height / 2));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.6f);

            ImGui.Begin("ProgressBackground", NoDecoration);
            ImGui.PopStyleVar();

            inner();
            ImGui.End();
        }

        public void DisplayProgress(Rectangle current, Action<Rectangle?> resultCallback)
        {
            throw new NotImplementedException();
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