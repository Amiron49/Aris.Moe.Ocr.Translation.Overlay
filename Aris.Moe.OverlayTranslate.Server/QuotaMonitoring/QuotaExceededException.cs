using System;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException(string type) : base($"reached the maximum supported quota for '{type}'")
        {
        }
    }
}