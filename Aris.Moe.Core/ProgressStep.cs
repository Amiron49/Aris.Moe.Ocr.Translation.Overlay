using System;

namespace Aris.Moe.Core
{
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