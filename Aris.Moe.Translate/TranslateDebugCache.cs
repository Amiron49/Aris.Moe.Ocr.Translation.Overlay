using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aris.Moe.Translate
{
    public class TranslateDebugCache : ITranslate
    {
        private readonly ITranslate _decorated;
        private readonly ITranslateConfig _configuration;

        public TranslateDebugCache(ITranslate decorated, ITranslateConfig configuration)
        {
            _decorated = decorated;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Translation>> Translate(IEnumerable<string> originals, string? targetLanguage = "en", string? inputLanguage = null)
        {
            if (!_configuration.Cache)
                return await _decorated.Translate(originals, targetLanguage, inputLanguage);

            var cached = GetCached();

            if (cached != null) 
                return cached;
            
            cached = (await _decorated.Translate(originals, targetLanguage, inputLanguage)).ToList();
            Cache(cached);

            return cached;
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