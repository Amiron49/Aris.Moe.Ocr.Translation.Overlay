namespace Aris.Moe.OverlayTranslate.Server.Error
{
    public class HashMismatchError : FluentResults.Error
    {
        public HashMismatchError(byte[] expectedHash, byte[] actualHash) : base($"Hash mismatch: Hash of image should have been {expectedHash} but was {actualHash}. " +
                                                                                $"It may indicate that the target image may not be public or the (Honyaku-chan) server IP has been banned")
        {
        }
    }
}