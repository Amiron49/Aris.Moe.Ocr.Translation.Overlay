using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Ocr
{
    public class OcrService
    {
        private readonly ISpatialTextConsolidator _spatialTextConsolidator;
        private readonly IMachineOcrRepository _machineOcrRepository;
        private readonly IOcr _ocr;

        public OcrService(ISpatialTextConsolidator spatialTextConsolidator)
        {
            _spatialTextConsolidator = spatialTextConsolidator;
        }

        public async Task<ConsolidatedMachineAddressableOcr> MachineOcrImage(ImageReference reference, Stream content)
        {
            var (texts, language) = await _ocr.Ocr(content);
            var spatialTexts = texts as ISpatialText[] ?? texts.ToArray();

            var rawMachineOcr = new RawMachineOcr
            {
                Created = DateTime.UtcNow,
                Provider = MachineOcrProvider.Google,
                Texts = spatialTexts,
                ForImage = reference.Id,
                Language = language
            };

            rawMachineOcr = await _machineOcrRepository.Save(rawMachineOcr);

            var consolidated = CreateAddressableMachineOcr(rawMachineOcr);

            return await _machineOcrRepository.Save(consolidated);
        }

        private ConsolidatedMachineAddressableOcr CreateAddressableMachineOcr(RawMachineOcr rawMachineOcr)
        {
            var consolidated = _spatialTextConsolidator.Consolidate(rawMachineOcr.Texts).Select(x => new AddressableSpatialText(x.Text, x.Area));

            var consolidatedMachineOcr = new ConsolidatedMachineAddressableOcr
            {
                RawMachineOcrId = rawMachineOcr.Id!.Value,
                Consolidation = ConsolidationMode.Default,
                Provider = rawMachineOcr.Provider,
                Language = rawMachineOcr.Language,
                Texts = consolidated,
                ForImage = rawMachineOcr.ForImage
            };

            return consolidatedMachineOcr;
        }

        public Task<IEnumerable<ConsolidatedMachineAddressableOcr>> GetConsolidatedMachineOcr(Guid knownImageId)
        {
            return _machineOcrRepository.GetConsolidated(knownImageId);
        }
    }
}