namespace Aris.Moe.OverlayTranslate.Server.ViewModel
{
    public interface IApiRequest
    {
        /// <summary>
        /// Optional ApiKey for tracking usage quota 
        /// </summary>
        public string? ApiKey { get; set; }
    }
}