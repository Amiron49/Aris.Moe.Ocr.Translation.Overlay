using System;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public static class HashHelper
    {
        public static bool ByteEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            return a.SequenceEqual(b);
        }

        public static string ToHexString(byte[] hex)
        {
            return BitConverter.ToString(hex);
        }
    }
}