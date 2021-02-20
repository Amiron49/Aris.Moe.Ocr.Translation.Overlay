namespace Aris.Moe.Configuration
{
    public interface IDeeplConfiguration
    {
        public string? ApiKey { get; set; }
        public bool UseFreeProxy { get; set; } 
    }
}