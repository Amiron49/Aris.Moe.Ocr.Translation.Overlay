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
            //var bmpScreenshot = new NativeWindowsScreenShoot().CaptureRectangle(new Rectangle(0,0,800,600));
            
             var bmpScreenshot = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
            
             var memoryStream = new MemoryStream();
            //
             using var g = Graphics.FromImage(bmpScreenshot);
            //
             g.CopyFromScreen(area.Location, new Point(0, 0), area.Size, CopyPixelOperation.SourceCopy);

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);  
                
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            Encoder myEncoder =
                Encoder.Quality;

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            //bmpScreenshot.Save(@"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private\debug-1.jpg", ImageFormat.Jpeg);
            bmpScreenshot.Save(memoryStream, ImageFormat.Png);

            return (memoryStream, bmpScreenshot);
        }
        
        private ImageCodecInfo GetEncoder(ImageFormat format)  
        {  
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();  
            foreach (ImageCodecInfo codec in codecs)  
            {  
                if (codec.FormatID == format.Guid)  
                {  
                    return codec;  
                }  
            }  
            throw new Exception(":D");  
        }  
    }
}

