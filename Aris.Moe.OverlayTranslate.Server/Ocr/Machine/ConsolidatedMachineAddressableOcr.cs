using System;
using System.Collections.Generic;
using Aris.Moe.Ocr;
using Aris.Moe.OverlayTranslate.Server.SpatialText;

namespace Aris.Moe.OverlayTranslate.Server.Ocr.Machine
{
    public class ConsolidatedMachineAddressableOcr: AddressableOcr<AddressableSpatialText>
    {
        public int? Id { get; init; }
        public int RawMachineOcrId { get; init; }
        public MachineOcrProvider Provider { get; }
        public ConsolidationMode Consolidation { get; }

        public ConsolidatedMachineAddressableOcr(string language, MachineOcrProvider provider, ConsolidationMode consolidation, IEnumerable<AddressableSpatialText> texts) : base(language, texts)
        {
            Consolidation = consolidation;
            Provider = provider;
        }
    }
    
    public class RawMachineOcr
    {
        public Guid? ForImage { get; init; }
        public string Language { get; }
        public int? Id { get; init; }
        public MachineOcrProvider Provider { get; }
        public DateTime Created { get; }
        public IEnumerable<ISpatialText> Texts { get; }
        
        public RawMachineOcr(string language, MachineOcrProvider provider, DateTime created, IEnumerable<ISpatialText> texts)
        {
            Language = language;
            Provider = provider;
            Created = created;
            Texts = texts;
        }
    }
}