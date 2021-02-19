using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Alturos.Yolo;
using Alturos.Yolo.Model;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class RegionsOfInterestFinder
    {
        private readonly ILogger _logger;
        private readonly IProgressOverlay _progressOverlay;
        private readonly Lazy<Task<YoloWrapper>> _lazyWrapper;
        private Task<YoloWrapper> Wrapper => _lazyWrapper.Value;

        private class YoloFile
        {
            public string ExpectedPath { get; }
            public string OnlineUrl { get; }
            public string Name { get; }

            public YoloFile(string rootPath, string name)
            {
                const string baseUrl = "https://s3.eu-central-1.amazonaws.com/aris.moe/yolo-manga/v1/";
                ExpectedPath = Path.Combine(rootPath, name);
                OnlineUrl = $"{baseUrl}{name}";
                Name = name;
            }
        }

        public RegionsOfInterestFinder(ILogger logger, IProgressOverlay progressOverlay)
        {
            _logger = logger;
            _progressOverlay = progressOverlay;
            _lazyWrapper = new Lazy<Task<YoloWrapper>>(InitYolo);
        }

        private async Task<YoloWrapper> InitYolo()
        {
            _logger.LogInformation("Init, Yolo");

            var currentDir = Directory.GetCurrentDirectory();
            var targetDir = Path.Combine(currentDir, "yolo");

            var cfg = new YoloFile(targetDir, "yolo-manga.cfg");
            var weights = new YoloFile(targetDir, "yolo-manga.weights");
            var names = new YoloFile(targetDir, "yolo-manga.names");

            var files = new[]
            {
                cfg,
                weights,
                names
            };

            var haveAllFiles = Directory.Exists(targetDir) && files.All(x => File.Exists(x.ExpectedPath));

            if (haveAllFiles)
                return new YoloWrapper(cfg.ExpectedPath, weights.ExpectedPath, names.ExpectedPath);

            foreach (var yoloFile in files)
            {
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                IfExistsDelete(yoloFile.ExpectedPath);
            }

            await DownloadAll(files);

            return new YoloWrapper(cfg.ExpectedPath, weights.ExpectedPath, names.ExpectedPath);
        }

        private async Task DownloadAll(IEnumerable<YoloFile> yoloFiles)
        {
            using var httpClient = new HttpClient();

            foreach (var yoloFile in yoloFiles)
                await DownloadSingle(httpClient, yoloFile);
        }

        private async Task DownloadSingle(HttpClient client, YoloFile file)
        {
            var progress = new Progress<double>();
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var response = await client.GetAsync(file.OnlineUrl, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);
                var size = response.Content.Headers.ContentLength;

                if (size == null)
                    throw new Exception("Couldn't get content length");

                using var httpStream = new StreamProgressTracker(await response.Content.ReadAsStreamAsync(), progress, size!.Value);
                using var fileStream = new FileStream(file.ExpectedPath, FileMode.Create);

                _progressOverlay.DisplayProgress($"Downloading Yolo files", cancellationTokenSource, new ProgressStep(file.Name, progress));

                await httpStream.CopyToAsync(fileStream);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to download Yolo File {file}", file.Name);
                cancellationTokenSource.Cancel();
            }
        }

        private static void IfExistsDelete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private enum RegionType
        {
            Text,
            Box,
            Bubble
        }

        public async Task<IEnumerable<Rectangle>> GetInterestRegions(Stream image)
        {
            var allRegions = await GetAllRegions(image);

            var groupedByOverlap = allRegions.GroupBy(x => x.Area, new OverlapComparer());

            var consolidated = groupedByOverlap.Select(group =>
            {
                var texts = group.Where(x => x.Type == RegionType.Text).ToList();

                if (texts.Any())
                    return texts.Select(x => x.Area).Aggregate(Rectangle.Union);

                return group.Select(x => x.Area).Aggregate(Rectangle.Union);
            });

            return consolidated;
        }

        private async Task<IEnumerable<(Rectangle Area, RegionType Type)>> GetAllRegions(Stream image)
        {
            var memoryStream = await StreamHelper.CreateStreamCopy(image);

            var items = (await Wrapper).Detect(memoryStream.ToArray());

            return items.Select(x => (new Rectangle(x.X, x.Y, x.Width, x.Height), Convert(x.Type)));
        }

        private static RegionType Convert(string type)
        {
            return type switch
            {
                "free-text" => RegionType.Text,
                "box" => RegionType.Box,
                "bubble" => RegionType.Bubble,
                _ => throw new ArgumentOutOfRangeException($"{type} is not known")
            };
        }
    }
}