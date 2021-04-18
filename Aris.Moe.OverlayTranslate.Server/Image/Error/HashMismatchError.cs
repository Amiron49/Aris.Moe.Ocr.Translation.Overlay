using System;
using System.Linq;
using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;

namespace Aris.Moe.OverlayTranslate.Server.Image.Error
{
    public class HashMismatchError : CorrelatedError
    {
        public HashMismatchError(byte[] expectedHash, byte[] actualHash)
        {
            var expectedBase64 = expectedHash.Any() ? Convert.ToBase64String(expectedHash) : "nothing";
            var actualHashBase64 = actualHash.Any() ? Convert.ToBase64String(actualHash) : "nothing";

            Message = $"Hash mismatch: Hash of image should have been [{expectedBase64}] but was [{actualHashBase64}]. " +
                      "It may indicate that the target image may not be public or the (Honyaku-chan) server IP has been banned";
        }
    }
}