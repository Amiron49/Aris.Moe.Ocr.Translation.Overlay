using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.OverlayTranslate.Server.AspNetCore;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.Translate;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Lamar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Aris.Moe.OverlayTranslate.Server.IntegrationTests
{
    public class Sanity
    {
        [Fact]
        public async Task KindaWorks()
        {
            var tempDbFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".db");
            
            var container = new Container(x =>
            {
                x.IncludeRegistry(new ApiRegistry(new ApiConfiguration
                {
                    Database = new DatabaseConfiguration
                    {
                        ConnectionString = $"Data Source={tempDbFile}"
                    }
                }));
                x.For<ITranslate>().Use<DummyTranslate>();
                x.For<IOcr>().Use<DummyOcr>();
                x.For<IImageFetcher>().Use<DummyFetcher>();
                x.For(typeof(ILogger<>)).Use(typeof(NullLogger<>));
            });
            
            var allOnStartup = container.GetAllInstances<IOnStartup>();

            foreach (var onStartup in allOnStartup)
                await onStartup.OnStartup();

            var analyzer = container.GetInstance<IImageAnalyser>();
            var service = container.GetInstance<IOverlayTranslateServer>();
            
            ImageInfo testImageInfo;
            // ReSharper disable once UseAwaitUsing
            using (var dummyImage = DummyFetcher.DummyImage())
                testImageInfo = await analyzer.Analyse(dummyImage);

            var translationResult = await service.TranslatePublic(new PublicOcrTranslationRequest()
            {
                ImageHash = testImageInfo.Sha256Hash,
                ImageUrl = "lel"
            });

            translationResult.Should().NotBeNull().And.BeSuccess();
        }
    }

    public class DummyTranslate : ITranslate
    {
        public Task<IEnumerable<Translate.Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            var translations = new List<Translate.Translation>
            {
                new("Yay", "YAY")
            };
            return Task.FromResult(translations.AsEnumerable());
        }
    }

    public class DummyOcr : IOcr
    {
        public Task<(IEnumerable<ISpatialText> Texts, string Language)> Ocr(Stream image, string? inputLanguage = null)
        {
            var ocr = new List<ISpatialText>()
            {
                new Moe.Ocr.SpatialText("YAY", new Rectangle(10, 10, 10, 10))
            };
            return Task.FromResult((ocr.AsEnumerable(), "ja"));
        }
    }
    
    public class DummyFetcher : IImageFetcher
    {
        public Task<Result<Stream>> Get(string url)
        {
            return Task.FromResult(Result.Ok(DummyImage()));
        }

        public static Stream DummyImage()
        {
            return typeof(Sanity).Assembly.GetManifestResourceStream("Aris.Moe.OverlayTranslate.Server.IntegrationTests.test.jpg")!;
        }
    }
}