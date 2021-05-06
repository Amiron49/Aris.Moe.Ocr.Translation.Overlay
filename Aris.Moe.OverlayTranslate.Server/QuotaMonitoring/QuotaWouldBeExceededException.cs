using System;

namespace Aris.Moe.OverlayTranslate.Server.QuotaMonitoring
{
    public class QuotaWouldBeExceededException : Exception
    {
        public QuotaWouldBeExceededException(string type, long usage, double exceedAmount) : base($"another {usage} units would exceed the max '{type}' quota by {exceedAmount}€.")
        {
        }
    }
}