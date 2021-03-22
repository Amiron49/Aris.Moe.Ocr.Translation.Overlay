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
        public MachineOcrProvider Provider { get; init; }
        public ConsolidationMode Consolidation { get; init; }
    }
    
    public class RawMachineOcr
    {
        public Guid? ForImage { get; init; }
        public string Language { get; init; }
        public int? Id { get; set; }
        public MachineOcrProvider Provider { get; init; }
        public DateTime Created { get; init; }
        public IEnumerable<ISpatialText> Texts { get; init; }
    }
}