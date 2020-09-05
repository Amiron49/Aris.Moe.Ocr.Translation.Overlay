using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Aris.Moe.Ocr.Overlay.Translate.NativeCapture
{
    public class NativeWindowsScreenShoot
    {
        public Bitmap CaptureRectangle(Rectangle rect)
        {
            return CaptureRectangleNative(rect);
        }
        
        private Bitmap CaptureRectangleNative(Rectangle rect)
        {
            IntPtr handle = NativeMethods.GetDesktopWindow();
            return CaptureRectangleNative(handle, rect);
        }

        private Bitmap CaptureRectangleNative(IntPtr handle, Rectangle rect)
        {
            if (rect.Width == 0 || rect.Height == 0)
                throw new Exception("Invalid target");

            var hdcSrc = NativeMethods.GetWindowDC(handle);
            var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);
            var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, rect.Width, rect.Height, hdcSrc, rect.X, rect.Y, (uint)(NativeCopyPixelOperation.Wtf));


            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(handle, hdcSrc);
            Bitmap bmp = Image.FromHbitmap(hBitmap);
            NativeMethods.DeleteObject(hBitmap);

            bmp.Save(@"E:\Projects\Aris.Moe.Ocr.Overlay.Translate.Cli\.private\debug-original.bmp", ImageFormat.Bmp);
            
            return bmp;
        }
    }
}