using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageAnalyser : IImageAnalyser
    {
        public async Task<ImageInfo> Analyse(Stream imageStream)
        {
            // ReSharper disable once UseAwaitUsing
            using var hoistyBoisty = new MemoryStream();

            await imageStream.CopyToAsync(hoistyBoisty);

            if (hoistyBoisty.Length == 0)
                throw new Exception("Image stream is 0");

            byte[] shaHash;
            using (var sha256Hasher = SHA256.Create())
            {
                shaHash = await sha256Hasher.ComputeHashAsync(hoistyBoisty);
            }
            
            var (image, format) = await SixLabors.ImageSharp.Image.LoadWithFormatAsync<Rgba32>(new SixLabors.ImageSharp.Configuration
            {
                ReadOrigin = ReadOrigin.Begin,
            }, hoistyBoisty);

            using (image)
            {
                var width = image.Width;
                var height = image.Height;
                var mimeType = format.DefaultMimeType;
            
                var averageHasher = new AverageHash();
                var averageHash = averageHasher.Hash(image);

                var perceptualHasher = new PerceptualHash();
                var perceptualHash = perceptualHasher.Hash(image);
                var differenceHasher = new DifferenceHash();
                var differenceHash = differenceHasher.Hash(image);

                return new ImageInfo(shaHash, averageHash, differenceHash, perceptualHash, width, height, mimeType);
            }
        }
    }
}