using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr.Overlay.Translate.Ocr
{
    public class OcrDebugCache : IOcr
    {
        private readonly IOcr _decorated;
        private readonly IOcrTranslateOverlayConfiguration _configuration;

        public OcrDebugCache(IOcr decorated, IOcrTranslateOverlayConfiguration configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ISpatialText>> Ocr(Stream image, string? inputLanguage = null)
        {
            if (!_configuration.PermanentlyCacheExternalOcrResult)
                return await _decorated.Ocr(image, inputLanguage);

            var result = GetCached();

            if (result == null)
            {
                result = (await _decorated.Ocr(image, inputLanguage)).ToList();
                Cache(result);
            }

            return result;
        }

        private List<ISpatialText>? GetCached()
        {
            var cacheFilePath = _configuration.CacheFolderRoot + $@"{Path.DirectorySeparatorChar}ocr_cache.json";

            var cacheExists = File.Exists(cacheFilePath);

            if (!cacheExists)
                return null;

            var content = File.ReadAllText(cacheFilePath);

            if (string.IsNullOrEmpty(content))
                return null;

            var deserialized = JsonConvert.DeserializeObject<List<SpatialText>>(content);

            return new List<ISpatialText>(deserialized);
        }

        private void Cache(IEnumerable<ISpatialText> texts)
        {
            var cacheFilePath = _configuration.CacheFolderRoot + $@"{Path.DirectorySeparatorChar}ocr_cache.json";

            var serialised = JsonConvert.SerializeObject(texts, Formatting.Indented);

            File.WriteAllText(cacheFilePath, serialised);
        }
    }
}