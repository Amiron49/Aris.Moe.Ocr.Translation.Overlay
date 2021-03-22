using System;
using System.Collections.Generic;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Aris.Moe.OverlayTranslate.Server.Translation;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public class MachineTranslationModel
    {
        public int Id { get; set; }
        public ConsolidatedMachineOcrModel MachineOcr { get; set; }
        public int MachineOcrId { get; set; }
        public MachineTranslationProvider Provider { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<AddressableSpatialTextModel> Texts { get; set; } = new List<AddressableSpatialTextModel>();
    }
}