using Aris.Moe.OverlayTranslate.Server.Image.Error;

namespace Aris.Moe.OverlayTranslate.Server.Image
{
    public class ImageVerification
    {
        public FluentResults.Error? Verify(ImageReference imageReference, ImageVerificationCharacteristics verificationCharacteristics)
        {
            var imageInfo = imageReference.Info;

            if (verificationCharacteristics.Hash != null)
            {
                var hashMatch = HashHelper.ByteEqual(imageInfo.Sha256Hash, verificationCharacteristics.Hash);

                if (!hashMatch)
                    return new HashMismatchError(verificationCharacteristics.Hash, imageInfo.Sha256Hash);
            }

            var expectedHeight = verificationCharacteristics.Height;
            var expectedWidth = verificationCharacteristics.Width;
            
            if (expectedHeight != imageInfo.Height || expectedWidth != imageInfo.Width)
                return new ExpectedImageDimensionMismatchError(expectedWidth!.Value, expectedHeight!.Value, imageInfo.Width, imageInfo.Height);

            return null;
        }

        public FluentResults.Error? VerifyCharacteristics(ImageVerificationCharacteristics verificationCharacteristics)
        {
            if (verificationCharacteristics.Hash is {Length: > 1})
                return null;

            if (verificationCharacteristics.Height == null || verificationCharacteristics.Width == null)
                return new NoImageVerificationCharacteristicsError();

            return null;
        }
    }
}