using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr
{
    public class OcrDebugCache : IOcr
    {
        private readonly IOcr _decorated;
        private readonly IOcrConfig _configuration;

        public OcrDebugCache(IOcr decorated, IOcrConfig configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }

        public async Task<(IEnumerable<ISpatialText> Texts, string Language)> Ocr(Stream image, string? inputLanguage = null)
        {
            if (!_configuration.Cache)
                return await _decorated.Ocr(image, inputLanguage);

            var cached = GetCached();

            if (cached != null)
                return (cached, "en");
            
            var newResult = (await _decorated.Ocr(image, inputLanguage));
            Cache(newResult.Texts);
            
            return newResult;
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