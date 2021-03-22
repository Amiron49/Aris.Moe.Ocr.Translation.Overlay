using System;

namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching
{
    public class MaxImageLengthException : Exception
    {
        public MaxImageLengthException(string domain, long maxLength) : base($"request to a file from domain '{domain}' has exceeded the maxlength of '{maxLength / 1024 / 1024}' MB")
        {
            
        }
    }
}