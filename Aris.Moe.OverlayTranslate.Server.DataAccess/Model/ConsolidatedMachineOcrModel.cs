using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class ConsolidatedMachineOcrModel
    {
        public int? Id { get; set; }
        public RawMachineOcrModel Raw { get; set; }
        public int? RawId { get; set; }
        public ConsolidationMode Consolidation { get; set; }
        public IEnumerable<AddressableSpatialTextModel> Texts { get; set; } = new List<AddressableSpatialTextModel>();
    }
}