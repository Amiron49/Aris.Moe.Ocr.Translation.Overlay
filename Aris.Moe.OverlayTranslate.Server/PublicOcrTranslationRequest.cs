using System.ComponentModel.DataAnnotations;

namespace Aris.Moe.OverlayTranslate.Server
{
    /// <summary>
    /// Request for translating a publicly reachable image
    /// </summary>
    public class PublicOcrTranslationRequest : IHashLookup, IUrlLookup
    {
        /// <summary>
        /// Public reachable url of the image
        /// </summary>
        [Required]
        [MaxLength(4000)]
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// SHAH256 hash of the image for quick lookup
        /// </summary>
        [MaxLength(300)]
        public byte[]? ImageHash { get; set; }

        public int? Height { get; set; }
        public int? Width { get; set; }

        /// <summary>
        /// Optional ApiKey for accessing privileged features or special account bound usage quota
        /// </summary>
        [MaxLength(300)]
        public string? ApiKey { get; set; }
    }
}