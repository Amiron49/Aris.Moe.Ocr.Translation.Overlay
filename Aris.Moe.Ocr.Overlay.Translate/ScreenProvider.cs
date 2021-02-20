using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aris.Moe.OverlayTranslate.Core;
using Aris.Moe.ScreenHelpers;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class ScreenProvider : IScreenImageProvider
    {
        private readonly IScreenInformation _screenInformation;

        public ScreenProvider(IScreenInformation screenInformation)
        {
            _screenInformation = screenInformation;
        }
        
        public (Stream Stream, Bitmap Original) Get(Rectangle area)
        {
            var totalScreenArea = _screenInformation.ScreenArea;

            var targetAreaAdjusted = new Rectangle(new Point(area.X + totalScreenArea.X, area.Y + totalScreenArea.Y), area.Size);

            return GetWithScreenAdjusted(targetAreaAdjusted);
        }

        private static (Stream Stream, Bitmap Original) GetWithScreenAdjusted(Rectangle area)
        {
            var bmpScreenshot = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);

            var memoryStream = new MemoryStream();
            using var g = Graphics.FromImage(bmpScreenshot);
            g.CopyFromScreen(area.Location, new Point(0, 0), area.Size, CopyPixelOperation.SourceCopy);

            bmpScreenshot.Save(memoryStream, ImageFormat.Png);

            memoryStream.Position = 0;
            
            return (memoryStream, bmpScreenshot);
        }
    }
}