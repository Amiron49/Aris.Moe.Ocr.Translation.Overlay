using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Image.Error;
using Aris.Moe.OverlayTranslate.Server.Ocr;
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
            var quickLookupResult = await Lookup(request);

            if (quickLookupResult.Value != null)
                return quickLookupResult;

            var verificationCharacteristics = ImageVerificationCharacteristics.From(request);

            var verificationErrors = _imageVerification.VerifyCharacteristics(verificationCharacteristics);

            if (verificationErrors != null)
                return Result.Fail(verificationErrors);

            var requestedImageResult = await TryFetchImage(request.ImageUrl, verificationCharacteristics);

            if (requestedImageResult.IsFailed)
                return requestedImageResult.ToResult();

            var requestImage = requestedImageResult.Value!.Value!;

            await SaveImage(requestImage.Reference);
            
            if (!_serverOptions.ConsiderPerceptualHashes)
                return await ProcessNewImage(requestImage);

            return await WeighAgainstPerceptualSameImages(requestImage);
        }

        private async Task SaveImage(ImageReference requestImageReference)
        {
            var existingByHash = await _imageReferenceRepository.Get(requestImageReference.Info.Sha256Hash);
            if (existingByHash != null)
            {
                await _imageReferenceRepository.AddUrl(existingByHash.Id, requestImageReference.OriginalUrl!);
                return;
            }
            
            await _imageReferenceRepository.Save(requestImageReference);
        }

        private async Task<Result<OcrTranslateResponse?>> Lookup(PublicOcrTranslationRequest request)
        {
            if (request.ImageHash != null)
                return await HashLookup(request);

            return await UrlLookup(request);
        }

        public async Task<Result<OcrTranslateResponse?>> UrlLookup(IUrlLookup request)
        {
            var knownImageByUrl = await _imageReferenceRepository.Get(request.ImageUrl);

            if (knownImageByUrl == null)
                return Result.Ok<OcrTranslateResponse?>(null);

            var highestQuality = await GetHighestQuality(knownImageByUrl.Info.Sha256Hash);

            return await GetResponseForKnownImage(highestQuality!.Id, MatchType.Url);
        }

        public async Task<Result<OcrTranslateResponse?>> HashLookup(IHashLookup request)
        {
            var knownImage = await GetHighestQuality(request.ImageHash);

            if (knownImage == null)
                return Result.Ok<OcrTranslateResponse?>(null);

            return await GetResponseForKnownImage(knownImage.Id, MatchType.Hash);
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

        private async Task<Result<OcrTranslateResponse?>> ProcessNewImage((ImageReference Reference, Stream Content) requestImage)
        {
            var machineOcr = await _ocrService.MachineOcrImage(requestImage.Reference, requestImage.Content);

            if (machineOcr.Language == "en")
                return Result.Fail<OcrTranslateResponse?>("Image is already in english.");

            await _translationService.MachineTranslate(machineOcr);

            return await GetResponseForKnownImage(requestImage.Reference.Id, MatchType.New);
        }

        private async Task<Result<OcrTranslateResponse?>> WeighAgainstPerceptualSameImages((ImageReference Reference, Stream Content) requestImage)
        {
            var visualHashMatches = (await _imageReferenceRepository.GetAll(requestImage.Reference.Info)).Where(x => x.Id != requestImage.Reference.Id).ToList();

            if (!visualHashMatches.Any())
                return await ProcessNewImage(requestImage);

            var highestQualityMatch = visualHashMatches.OrderByDescending(x => x.QualityScore).First();
            var newFileIsHigherQuality = requestImage.Reference.QualityScore > highestQualityMatch.QualityScore;

            if (newFileIsHigherQuality)
                return await ProcessNewImage(requestImage);

            return await GetResponseForKnownImage(highestQualityMatch.Id, MatchType.VisualHash);
        }

        private async Task<Result<OcrTranslateResponse?>> GetResponseForKnownImage(Guid id, MatchType matchType)
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

        private async Task<Result<(ImageReference Reference, Stream Content)?>> TryFetchImage(string url, ImageVerificationCharacteristics verificationCharacteristics)
        {
            var fetchImageResult = await _imageFetcher.Get(url);

            if (fetchImageResult.IsFailed)
                return fetchImageResult.ToResult<(ImageReference Reference, Stream Content)?>();

            var imageFactoryResult = await _imageFactory.Create(url, fetchImageResult.Value);

            if (!imageFactoryResult.IsSuccess)
                return imageFactoryResult.ToResult();

            var image = imageFactoryResult.ValueOrDefault;

            if (image == null)
                return Result.Fail(new CouldntReachImageError());

            var verificationError = _imageVerification.Verify(image, verificationCharacteristics);

            if (verificationError == null)
                return Result.Ok<(ImageReference Reference, Stream Content)?>((image, fetchImageResult.Value));

            return Result.Fail(verificationError);
        }
    }
}