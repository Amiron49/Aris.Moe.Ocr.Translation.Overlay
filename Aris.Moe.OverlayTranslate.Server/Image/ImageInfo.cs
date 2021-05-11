namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageInfo
    {
        public byte[] Sha256Hash { get; }
        public ulong AverageHash { get; }
        public ulong DifferenceHash { get; }
        public ulong PerceptualHash { get; }
        public int Width { get; }
        public int Height { get; }
        public string MimeType { get; }

        public ImageInfo(byte[] sha256Hash, ulong averageHash, ulong differenceHash, ulong perceptualHash, int width, int height, string mimeType)
        {
            Sha256Hash = sha256Hash;
            AverageHash = averageHash;
            DifferenceHash = differenceHash;
            PerceptualHash = perceptualHash;
            Width = width;
            Height = height;
            MimeType = mimeType;
        }
    }
}