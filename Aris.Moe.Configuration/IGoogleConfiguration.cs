namespace Aris.Moe.Configuration
{
    public interface IGoogleConfiguration
    {
        public string? Key { get; set; }
        public string? KeyPath { get; set; }
        public string? ProjectId { get; set; } 
        public string? LocationId { get; set; }
    }
}