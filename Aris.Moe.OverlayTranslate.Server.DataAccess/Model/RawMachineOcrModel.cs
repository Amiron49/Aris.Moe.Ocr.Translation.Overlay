using System;
using System.Text.Json;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class RawMachineOcrModel
    {
        public int? Id { get; set; }
        public Guid ForImage { get; set; }
        public string Language { get; set; } = null!;
        public MachineOcrProvider Provider { get; set; }
        public DateTime Created { get; set; }
        //public IEnumerable<SpatialText> Texts { get; set; } = new List<SpatialText>();
        public JsonDocument Texts { get; set; } = null!;
    }
}