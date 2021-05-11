using System.Linq;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore
{
    public class ResultLogger: IResultLogger
    {
        private readonly ILogger<Result> _logger;

        public ResultLogger(ILogger<Result> logger)
        {
            _logger = logger;
        }
        
        public void Log(string context, ResultBase result)
        {
            if (result.IsSuccess)
                return;
            
            if (result.Errors?.Any() ?? false)
                _logger.LogWarning(result.ToString());
        }
    }
}