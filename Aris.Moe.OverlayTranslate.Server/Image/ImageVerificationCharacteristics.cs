namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public record ImageVerificationCharacteristics
    {
        public byte[]? Hash { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }

        public static ImageVerificationCharacteristics From(PublicOcrTranslationRequest request)
        {
            return new()
            {
                Hash = request.ImageHash,
                Height = request.Height,
                Width = request.Width
            };
        }
    }
}