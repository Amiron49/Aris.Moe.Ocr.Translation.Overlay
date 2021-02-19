namespace Aris.Moe.Ocr.Overlay.Translate
{
    public interface IDeeplConfiguration
    {
        public string? ApiKey { get; set; }
        public bool UseFreeProxy { get; set; } 
    }
}