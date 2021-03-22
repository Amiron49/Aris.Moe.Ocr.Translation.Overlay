using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;
using Aris.Moe.OverlayTranslate.Server.SpatialText;
using Aris.Moe.Translate;

namespace Aris.Moe.OverlayTranslate.Server.Translation
{
    public class TranslationService
    {
        private readonly ITranslate _translate;
        private readonly ITranslationRepository _translationRepository;

        public TranslationService(ITranslate translate, ITranslationRepository translationRepository)
        {
            _translate = translate;
            _translationRepository = translationRepository;
        }

        public async Task<MachineTranslation> MachineTranslate(ConsolidatedMachineAddressableOcr source)
        {
            var translated = await _translate.Translate(source.Texts.Select(x => x.Text), "en", source.Language);

            var machineTranslation = new MachineTranslation
            {
                Language = "en",
                Provider = MachineTranslationProvider.Deepl,
                Texts = ToSpatialTexts(source.Texts.ToList(), translated.ToList())
            };

            return await _translationRepository.SaveMachineTranslation(source.Id!.Value, machineTranslation);
        }

        private static IEnumerable<BasedOnSpatialText> ToSpatialTexts(IList<AddressableSpatialText> originals, IList<Translate.Translation> translations)
        {
            for (var i = 0; i < originals.Count; i++)
            {
                var original = originals[i];
                var translation = translations[i];

                yield return new BasedOnSpatialText(translation.Text, original.Area)
                {
                    BasedOn = original.Id,
                    Created = DateTime.UtcNow,
                };
            }
        }

        public Task<IEnumerable<MachineTranslation>> GetAllMachineTranslations(Guid imageReferenceId)
        {
            return _translationRepository.GetMachineTranslations(imageReferenceId);
        }
    }
}