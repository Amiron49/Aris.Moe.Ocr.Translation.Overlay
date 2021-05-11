using Aris.Moe.OverlayTranslate.Server.Image.Fetching.Error;

namespace Aris.Moe.OverlayTranslate.Server.Image.Error
{
    public class ExpectedImageDimensionMismatchError : CorrelatedError
    {
        public ExpectedImageDimensionMismatchError(int expectedWidth, int expectedHeight, int actualWidth, int actualHeight)
        {
            Message = $"The fetched image dimensions '{actualWidth}x{actualHeight}' is not equal to the expected image dimensions '{expectedWidth}x{expectedHeight}'";
            Metadata.Add(nameof(actualWidth), actualWidth);
            Metadata.Add(nameof(actualHeight), actualHeight);
        }
    }
}