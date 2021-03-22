using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Error;
using Aris.Moe.OverlayTranslate.Server.Image;
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

        public OverlayTranslateServer(IImageReferenceRepository imageReferenceRepository, IImageFetcher imageFetcher, IImageFactory imageFactory, OcrService ocrService,
            TranslationService translationService)
        {
            _imageReferenceRepository = imageReferenceRepository;
            _imageFetcher = imageFetcher;
            _imageFactory = imageFactory;
            _ocrService = ocrService;
            _translationService = translationService;
        }

        public async Task<Result<OcrTranslateResponse?>> Lookup(IHashLookup request)
        {
            var knownImage = await GetHighestQuality(request.ImageHash);

            if (knownImage == null)
                return Result.Ok<OcrTranslateResponse?>(null);

            return await GetResponseForKnownImage(knownImage.Id);
        }

        /// <summary>
        /// Attempts to get highest quality version of an image hash
        /// </summary>
        private async Task<ImageReference?> GetHighestQuality(byte[] hash)
        {
            var knownImage = await _imageReferenceRepository.Get(hash);

            if (knownImage == null)
                return null;

            var allRelated = (await _imageReferenceRepository.GetAll(knownImage.Info)).ToList();

            return allRelated.OrderByDescending(x => x.QualityScore).First();
        }

        private async Task<Result<OcrTranslateResponse?>> GetResponseForKnownImage(Guid id)
        {
            var imageReference = await _imageReferenceRepository.Get(id);

            var machineOcr = (await _ocrService.GetConsolidatedMachineOcr(imageReference!.Id)).ToList();

            if (machineOcr!.First().Language == "en")
                return Result.Fail<OcrTranslateResponse?>("Image is already in english.");

            var machineTranslations = await _translationService.GetAllMachineTranslations(imageReference.Id);

            var ocrTranslateResponse = new OcrTranslateResponse
            {
                Match = MatchType.Hash,
                Image = imageReference.Info,
                MachineOcrs = machineOcr.Select(x => x.ToOcrViewModel()),
                MachineTranslations = machineTranslations.Select(x => x.ToTranslationViewModel())
            };

            return Result.Ok<OcrTranslateResponse?>(ocrTranslateResponse);
        }

        public async Task<Result<OcrTranslateResponse?>> TranslatePublic(PublicOcrTranslationRequest request)
        {
            var fromHash = await Lookup(request);

            if (fromHash.Value != null)
                return fromHash;

            var requestedImageResult = await TryFetchImage(request.ImageUrl, request.ImageHash);

            if (requestedImageResult.IsFailed)
                return requestedImageResult.ToResult();

            var requestImage = requestedImageResult.Value!.Value!;
            await _imageReferenceRepository.Save(requestImage.Reference);

            var visualHashMatches = (await _imageReferenceRepository.GetAll(requestImage.Reference.Info)).Where(x => x.Id != requestImage.Reference.Id).ToList();

            if (!visualHashMatches.Any())
                return await ProcessNewImage(requestImage);

            var highestQualityMatch = visualHashMatches.OrderByDescending(x => x.QualityScore).First();
            var newFileIsHigherQuality = requestImage.Reference.QualityScore > highestQualityMatch.QualityScore;

            if (newFileIsHigherQuality)
                return await ProcessNewImage(requestImage);

            return await GetResponseForKnownImage(highestQualityMatch.Id);
        }

        private async Task<Result<OcrTranslateResponse?>> ProcessNewImage((ImageReference Reference, Stream Content) requestImage)
        {
            var machineOcr = await _ocrService.MachineOcrImage(requestImage.Reference, requestImage.Content);

            if (machineOcr.Language == "en")
                return Result.Fail<OcrTranslateResponse?>("Image is already in english.");

            await _translationService.MachineTranslate(machineOcr);

            return await GetResponseForKnownImage(requestImage.Reference.Id);
        }

        private async Task<Result<(ImageReference Reference, Stream Content)?>> TryFetchImage(string url, byte[] expectedHash)
        {
            const int retryAmount = 1;

            ImageReference? image = null;
            for (var i = 0; i <= retryAmount; i++)
            {
                var fetchImageResult = await _imageFetcher.Get(url);

                if (fetchImageResult.IsFailed)
                    return fetchImageResult.ToResult<(ImageReference Reference, Stream Content)?>();

                image = await _imageFactory.Create(url, fetchImageResult.Value);

                if (image != null && HashHelper.ByteEqual(expectedHash, image!.Info.Sha256Hash))
                    return Result.Ok<(ImageReference Reference, Stream Content)?>((image, fetchImageResult.Value));
            }

            if (image == null)
                return Result.Fail(new CouldntReachImageError());

            return Result.Fail(new HashMismatchError(expectedHash, image.Info.Sha256Hash));
        }
    }

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