namespace Aris.Moe.Ocr.Overlay.Translate
{
    public interface IGoogleConfiguration
    {
        public string? KeyPath { get; set; }
        public string? ProjectId { get; set; } 
        public string? LocationId { get; set; }
    }
}