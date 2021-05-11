using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    [Owned]
    public class ImageInfoModel
    {
        public byte[] Sha256Hash { get; set; } = null!;
        public ulong AverageHash { get; set; }
        public ulong DifferenceHash { get; set; }
        public ulong PerceptualHash { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MimeType { get; set; } = null!;
    }
}