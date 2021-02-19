using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Aris.Moe.Ocr.Overlay.Translate.Core
{
    public interface IOverlay : ITextOverlay, ITargetAreaResizeOverlay , IDisposable
    {
        public bool Ready { get; }
        public void HideOverlay();
        public void ShowOverlay();
        public void ToggleOverlay();
        public Task Init();
    }

    public interface ITextOverlay
    {
        void Add(params ISpatialText[] texts);
        void ClearAll();
    }

    public interface ITargetAreaResizeOverlay
    {
        void AskForResize(Rectangle current, Action<Rectangle?> resultCallback);
    }
    
    public interface IProgressOverlay
    {
        void DisplayProgress(string description, CancellationTokenSource cancellationTokenSource, ProgressStep step, params ProgressStep[] moreSteps);
    }
    

    public class ProgressStep    
    {
        public string StepDescription { get; }
        public double ProgressPercentage { get; private set; }
        public event EventHandler? OnFinished;
        public ProgressStep(string stepDescription, Progress<double> progress)
        {
            StepDescription = stepDescription;
            progress.ProgressChanged += (sender, d) =>
            {
                ProgressPercentage = d;

                if (d >= 1)
                    OnFinished?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}