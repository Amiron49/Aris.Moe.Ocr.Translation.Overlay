using System;
using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class AddressableSpatialTextModel
    {
        public int? Id { get; set; }
        public int? BasedOnSpatialOcrText { get; set; }
        public int? MachineOcrId { get; set; }
        public ConsolidatedMachineOcrModel? MachineOcr { get; set; }
        public int? MachineTranslationId { get; set; }
        public int? UserId { get; set; }
        public string Text { get; set; } = null!;
        public string? Language { get; set; }
        public DateTime Created { get; set; }
        public RectangleModel Rectangle { get; set; } = null!;
    }
}