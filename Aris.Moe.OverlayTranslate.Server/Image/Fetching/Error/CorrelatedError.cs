using System;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class CorrelatedError : FluentResults.Error
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        public CorrelatedError()
        {
        }
        
        public CorrelatedError(string message) : base(message)
        {
            Message += $"If you think this is an error on our side, please make a bug report with this correlationID: {_correlationId}";
            Metadata.Add("Correlation", _correlationId);
        }
    }
}