using Aris.Moe.Configuration;

namespace Aris.Moe.OverlayTranslate.Configuration
{
    public class DeeplConfiguration : IDeeplConfiguration
    {
        public string? ApiKey { get; set; }
        public bool UseFreeProxy { get; set; } = false;
    }
}