using System.ComponentModel.DataAnnotations;

namespace Aris.Moe.OverlayTranslate.Server
{
    /// <summary>
    /// Request for translating a publicly reachable image
    /// </summary>
    public class PublicOcrTranslationRequest : IHashLookup
    {
        /// <summary>
        /// Public reachable url of the image
        /// </summary>
        [Required]
        [MaxLength(4000)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// SHAH256 hash of the image for quick lookup
        /// </summary>
        [Required]
        [MaxLength(300)]
        public byte[] ImageHash { get; set; }

        /// <summary>
        /// Optional ApiKey for accessing privileged features or special account bound usage quota
        /// </summary>
        [MaxLength(300)]
        public string? ApiKey { get; set; }
    }
}