using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Image.Error;
using Aris.Moe.OverlayTranslate.Server.Ocr;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;
using Aris.Moe.OverlayTranslate.Server.Translation;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using FluentResults;
using MatchType = Aris.Moe.OverlayTranslate.Server.Image.MatchType;

namespace Aris.Moe.OverlayTranslate.Server
{
    public class OverlayTranslateServer : IOverlayTranslateServer
    {
        private readonly IImageReferenceRepository _imageReferenceRepository;
        private readonly IImageFetcher _imageFetcher;
        private readonly IImageFactory _imageFactory;
        private readonly OcrService _ocrService;
        private readonly TranslationService _translationService;
        private readonly ImageVerification _imageVerification;
        private readonly OverlayTranslateServerOptions _serverOptions = new();

        public OverlayTranslateServer(IImageReferenceRepository imageReferenceRepository, IImageFetcher imageFetcher, IImageFactory imageFactory, OcrService ocrService,
            TranslationService translationService, ImageVerification imageVerification)
        {
            _imageReferenceRepository = imageReferenceRepository;
            _imageFetcher = imageFetcher;
            _imageFactory = imageFactory;
            _ocrService = ocrService;
            _translationService = translationService;
            _imageVerification = imageVerification;
        }

        public async Task<Result<OcrTranslateResponse?>> TranslatePublic(PublicOcrTranslationRequest request)
        {
            var verificationCharacteristics = ImageVerificationCharacteristics.From(request);
            var verificationErrors = _imageVerification.VerifyCharacteristics(verificationCharacteristics);

            if (verificationErrors != null)
                return Result.Fail(verificationErrors);

            var imageReferenceMatch = await LookupImage(request);
            var status = await GetImageStatus(imageReferenceMatch?.reference);

            if (imageReferenceMatch != null && status.HasOcr && status.HasTranslation)
                return await GetResponseForCompletelyProcessedImage(imageReferenceMatch.Value.reference.Id, imageReferenceMatch.Value.type);

            if (imageReferenceMatch == null)
            {
                if (request.ImageHash != null)
                    return await TranslatePublicByHash(request, verificationCharacteristics);

                return await TranslatePublicByUrlOnly(request, verificationCharacteristics);
            }
            
            return await HandleIncompleteKnownImage(imageReferenceMatch.Value.reference, request.ImageUrl, verificationCharacteristics, imageReferenceMatch.Value.type);
        }

        private async Task<Result<OcrTranslateResponse?>> TranslatePublicByHash(PublicOcrTranslationRequest request, ImageVerificationCharacteristics verificationCharacteristics)
        {
            var requestedImageResult = await TryFetchImage(request.ImageUrl, verificationCharacteristics);

            if (requestedImageResult.IsFailed)
                return requestedImageResult.ToResult();

            var (reference, content) = requestedImageResult.Value;

            return await HandleNewImage(reference, content);
        }

        private async Task<Result<OcrTranslateResponse?>> TranslatePublicByUrlOnly(PublicOcrTranslationRequest request, ImageVerificationCharacteristics verificationCharacteristics)
        {
            var requestedImageResult = await TryFetchImage(request.ImageUrl, verificationCharacteristics);

            if (requestedImageResult.IsFailed)
                return requestedImageResult.ToResult();

            var (reference, content) = requestedImageResult.Value;

            var knownMatchingImage = await _imageReferenceRepository.Get(requestedImageResult.Value.Reference.Info.Sha256Hash);

            if (knownMatchingImage == null)
                return await HandleNewImage(reference, content);

            await _imageReferenceRepository.AddUrl(knownMatchingImage.Id, request.ImageUrl);

            return await HandlePotentiallyIncompleteKnownImage(knownMatchingImage, content, MatchType.Hash);
        }

        private async Task<Result<OcrTranslateResponse?>> HandleNewImage(ImageReference reference, Stream content)
        {
            reference = await _imageReferenceRepository.Save(reference);
            var ocr = await _ocrService.MachineOcrImage(reference, content);
            await _translationService.MachineTranslate(ocr);

            return await GetResponseForCompletelyProcessedImage(reference.Id, MatchType.New);
        }

        private async Task<Result<OcrTranslateResponse?>> HandleIncompleteKnownImage(ImageReference reference, string url,
            ImageVerificationCharacteristics imageVerificationCharacteristics, MatchType matchType)
        {
            var requestedImageResult = await TryFetchImage(url, imageVerificationCharacteristics with {Hash = reference.Info.Sha256Hash});

            if (requestedImageResult.IsFailed)
                return requestedImageResult.ToResult();

            var (_, content) = requestedImageResult.Value;

            return await HandlePotentiallyIncompleteKnownImage(reference, content, matchType);
        }

        private async Task<Result<OcrTranslateResponse?>> HandlePotentiallyIncompleteKnownImage(ImageReference reference, Stream content, MatchType matchType)
        {
            var allOcr = (await _ocrService.GetConsolidatedMachineOcr(reference.Id)).ToList();

            if (!allOcr.Any())
                allOcr = new List<ConsolidatedMachineAddressableOcr> {await _ocrService.MachineOcrImage(reference, content)};

            var anyTranslations = await _translationService.AnyTranslations(reference.Id);

            if (!anyTranslations)
                await _translationService.MachineTranslate(allOcr.First(x => x.Provider == MachineOcrProvider.Google));

            return await GetResponseForCompletelyProcessedImage(reference.Id, matchType);
        }

        public async Task<Result<OcrTranslateResponse?>> UrlLookup(IUrlLookup request)
        {
            var knownImageByUrl = await _imageReferenceRepository.Get(request.ImageUrl);

            if (knownImageByUrl == null)
                return Result.Ok<OcrTranslateResponse?>(null);

            var highestQuality = await GetHighestQuality(knownImageByUrl.Info.Sha256Hash);

            return await GetResponseForCompletelyProcessedImage(highestQuality!.Id, MatchType.Url);
        }

        public async Task<(ImageReference reference, MatchType type)?> LookupImage(PublicOcrTranslationRequest request)
        {
            if (request.ImageHash != null)
            {
                var hashMatch = await _imageReferenceRepository.Get(request.ImageHash);
                if (hashMatch == null)
                    return null;

                return (hashMatch, MatchType.Hash);
            }

            var urlMatch = await _imageReferenceRepository.Get(request.ImageUrl);
            if (urlMatch == null)
                return null;

            return (urlMatch, MatchType.Url);
        }

        public async Task<ImageStatus> GetImageStatus(ImageReference? reference)
        {
            if (reference == null)
            {
                return new ImageStatus
                {
                    HasOcr = false,
                    HasTranslation = false,
                    IsKnown = false
                };
            }

            var hasOcr = await _ocrService.AnyMachineOcr(reference.Id);

            if (!hasOcr)
            {
                return new ImageStatus
                {
                    IsKnown = true,
                    HasOcr = false,
                    HasTranslation = false
                };
            }

            var hasTranslation = await _translationService.AnyTranslations(reference.Id);

            return new ImageStatus
            {
                IsKnown = true,
                HasOcr = true,
                HasTranslation = hasTranslation
            };
        }

        public async Task<Result<OcrTranslateResponse?>> HashLookup(IHashLookup request)
        {
            var knownImage = await GetHighestQuality(request.ImageHash);

            if (knownImage == null)
                return Result.Ok<OcrTranslateResponse?>(null);

            return await GetResponseForCompletelyProcessedImage(knownImage.Id, MatchType.Hash);
        }

        /// <summary>
        /// Attempts to get highest quality version of an image hash
        /// </summary>
        private async Task<ImageReference?> GetHighestQuality(byte[] hash)
        {
            var knownImage = await _imageReferenceRepository.Get(hash);

            if (!_serverOptions.ConsiderPerceptualHashes)
                return knownImage;

            if (knownImage == null)
                return null;

            var allRelated = (await _imageReferenceRepository.GetAll(knownImage.Info)).ToList();

            return allRelated.OrderByDescending(x => x.QualityScore).First();
        }

        private async Task<Result<OcrTranslateResponse?>> TranslateImage((ImageReference Reference, Stream Content) requestImage)
        {
            var machineOcr = await _ocrService.MachineOcrImage(requestImage.Reference, requestImage.Content);

            if (machineOcr.Language == "en")
                return Result.Fail<OcrTranslateResponse?>("Image is already in english.");

            await _translationService.MachineTranslate(machineOcr);

            return await GetResponseForCompletelyProcessedImage(requestImage.Reference.Id, MatchType.New);
        }

        private async Task<Result<OcrTranslateResponse?>> WeighAgainstPerceptualSameImages((ImageReference Reference, Stream Content) requestImage)
        {
            var visualHashMatches = (await _imageReferenceRepository.GetAll(requestImage.Reference.Info)).Where(x => x.Id != requestImage.Reference.Id).ToList();

            if (!visualHashMatches.Any())
                return await TranslateImage(requestImage);

            var highestQualityMatch = visualHashMatches.OrderByDescending(x => x.QualityScore).First();
            var newFileIsHigherQuality = requestImage.Reference.QualityScore > highestQualityMatch.QualityScore;

            if (newFileIsHigherQuality)
                return await TranslateImage(requestImage);

            return await GetResponseForCompletelyProcessedImage(highestQualityMatch.Id, MatchType.VisualHash);
        }

        private async Task<Result<OcrTranslateResponse?>> GetResponseForCompletelyProcessedImage(Guid id, MatchType matchType)
        {
            var imageReference = await _imageReferenceRepository.Get(id);

            var machineOcr = (await _ocrService.GetConsolidatedMachineOcr(imageReference!.Id)).ToList();

            if (machineOcr!.First().Language == "en")
                return Result.Fail<OcrTranslateResponse?>("Image is already in english.");

            var machineTranslations = await _translationService.GetAllMachineTranslations(imageReference.Id);

            var ocrViewModels = machineOcr.Select(x => x.ToOcrViewModel());
            var translationViewModels = machineTranslations.Select(x => x.ToTranslationViewModel());
            var ocrTranslateResponse = new OcrTranslateResponse(matchType, imageReference.Info, translationViewModels, ocrViewModels);

            return Result.Ok<OcrTranslateResponse?>(ocrTranslateResponse);
        }

        private async Task<Result<(ImageReference Reference, Stream Content)>> TryFetchImage(string url, ImageVerificationCharacteristics verificationCharacteristics)
        {
            var fetchImageResult = await _imageFetcher.Get(url);

            if (fetchImageResult.IsFailed)
                return fetchImageResult.ToResult<(ImageReference Reference, Stream Content)>();

            var imageFactoryResult = await _imageFactory.Create(url, fetchImageResult.Value);

            if (!imageFactoryResult.IsSuccess)
                return imageFactoryResult.ToResult();

            var image = imageFactoryResult.ValueOrDefault;

            if (image == null)
                return Result.Fail(new CouldntReachImageError());

            var verificationError = _imageVerification.Verify(image, verificationCharacteristics);

            if (verificationError == null)
                return Result.Ok<(ImageReference Reference, Stream Content)>((image, fetchImageResult.Value));

            return Result.Fail(verificationError);
        }
    }

    public class ImageStatus
    {
        public bool IsKnown { get; set; }
        public bool HasOcr { get; set; }
        public bool HasTranslation { get; set; }
    }
}