﻿namespace Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error
{
    public class RejectionStatusCodeError : CorrelatedError
    {
        public RejectionStatusCodeError(int statusCode) : base(
            $"Failed to load the image due to the server rejecting the request ({statusCode}). Are you sure that the image is publicly reachable?")
        {
        }
    }
}