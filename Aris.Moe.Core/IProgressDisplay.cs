using System.Threading;

namespace Aris.Moe.Core
{
    public interface IProgressDisplay
    {
        void DisplayProgress(string description, CancellationTokenSource cancellationTokenSource, ProgressStep step, params ProgressStep[] moreSteps);
    }
}