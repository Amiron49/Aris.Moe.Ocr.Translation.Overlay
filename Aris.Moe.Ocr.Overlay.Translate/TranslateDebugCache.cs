using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr.Overlay.Translate.Core;
using Newtonsoft.Json;

namespace Aris.Moe.Ocr.Overlay.Translate
{
    public class TranslateDebugCache: ITranslate
    {
        private readonly ITranslate _decorated;
        private readonly IOcrTranslateOverlayConfiguration _configuration;

        public TranslateDebugCache(ITranslate decorated, IOcrTranslateOverlayConfiguration configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Translation>> Translate(IEnumerable<ISpatialText> spatialTexts, string? targetLanguage = "en", string? inputLanguage = null)
        {
            if (!_configuration.PermanentlyCacheExternalOcrResult)
                return await _decorated.Translate(spatialTexts, targetLanguage, inputLanguage);

            var result = GetCached();

            if (result == null)
            {
                result = (await _decorated.Translate(spatialTexts, targetLanguage, inputLanguage)).ToList();
                Cache(result);
            }

            return result;
        }
        
        private List<Translation>? GetCached()
        {
            var cacheFilePath = _configuration.CacheFolderRoot + $@"{Path.DirectorySeparatorChar}translate_cache.json";
            
            var cacheExists = File.Exists(cacheFilePath);

            if (!cacheExists)
                return null;

            var content = File.ReadAllText(cacheFilePath);

            if (string.IsNullOrEmpty(content))
                return null;
            
            var deserialized = JsonConvert.DeserializeObject<List<Translation>>(content);

            return new List<Translation>(deserialized);
        }

        private void Cache(IEnumerable<Translation> texts)
        {
            var cacheFilePath = _configuration.CacheFolderRoot + $@"{Path.DirectorySeparatorChar}translate_cache.json";
            
            var serialised = JsonConvert.SerializeObject(texts, Formatting.Indented);
            
            File.WriteAllText(cacheFilePath, serialised);
        }
    }
}