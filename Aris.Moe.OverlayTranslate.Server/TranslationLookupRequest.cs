using System.ComponentModel.DataAnnotations;

namespace Aris.Moe.OverlayTranslate.Server
{
    /// <summary>
    /// Hash based quick lookup for a translation
    /// </summary>
    public class TranslationLookupRequest : IHashLookup
    {
        [Required]
        public byte[] ImageHash { get; set; } = null!;

        public string? ApiKey { get; set; }
    }
}