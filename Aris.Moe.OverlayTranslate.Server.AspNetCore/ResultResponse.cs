using System.Linq;
using FluentResults;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore
{
    public class ResultResponse<T>
    {
        public T? Result { get; }
        public bool Success { get; }
        public string Message { get; }

        public ResultResponse(Result<T> result)
        {
            Result = result.ValueOrDefault;
            Success = result.IsSuccess && !result.IsFailed;

            if (result.Reasons?.Any() ?? false)
                Message = result.Reasons.First().Message;

            result.Log();
        }
    }
}