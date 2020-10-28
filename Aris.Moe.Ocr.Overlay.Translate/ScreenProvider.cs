using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aris.Moe.Ocr.Overlay.Translate.Core;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class ScreenProvider : IScreenImageProvider
    {
        public (Stream Stream, Bitmap Original) Get(Rectangle area)
        {
            var bmpScreenshot = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);

            var memoryStream = new MemoryStream();
            using var g = Graphics.FromImage(bmpScreenshot);
            g.CopyFromScreen(area.Location, new Point(0, 0), area.Size, CopyPixelOperation.SourceCopy);

            bmpScreenshot.Save(memoryStream, ImageFormat.Png);

            return (memoryStream, bmpScreenshot);
        }
    }
}