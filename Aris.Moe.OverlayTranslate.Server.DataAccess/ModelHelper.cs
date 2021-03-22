using System;
using System.Linq;
using System.Text.Json;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Ocr.Machine;
using Aris.Moe.OverlayTranslate.Server.SpatialText;
using Aris.Moe.OverlayTranslate.Server.Translation;
using Aris.Moe.OverlayTranslate.Server.ViewModel;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public static class ModelHelper
    {
        public static ImageReference ToBusinessModel(this ImageReferenceModel model)
        {
            return new ImageReference
            {
                Id = model.Id,
                Info = model.Info.ToBusinessModel(),
                OriginalUrl = model.OriginalUrl,
                QualityScore = model.QualityScore
            };
        }

        public static ImageReferenceModel ToModel(this ImageReference model)
        {
            return new ImageReferenceModel
            {
                Id = model.Id,
                Info = model.Info.ToModel(),
                OriginalUrl = model.OriginalUrl,
                QualityScore = model.QualityScore
            };
        }

        public static ImageInfo ToBusinessModel(this ImageInfoModel model)
        {
            return new ImageInfo(model.Sha256Hash, model.AverageHash, model.DifferenceHash, model.PerceptualHash, model.Width, model.Height, model.MimeType);
        }

        public static ImageInfoModel ToModel(this ImageInfo model)
        {
            return new ImageInfoModel
            {
                Height = model.Height,
                Width = model.Width,
                AverageHash = model.AverageHash,
                DifferenceHash = model.DifferenceHash,
                MimeType = model.MimeType,
                PerceptualHash = model.PerceptualHash,
                Sha256Hash = model.Sha256Hash
            };
        }

        public static ConsolidatedMachineAddressableOcr ToBusinessModel(this ConsolidatedMachineOcrModel model)
        {
            return new()
            {
                Id = model.Id,
                Consolidation = model.Consolidation,
                Language = model.Raw.Language,
                Provider = model.Raw.Provider,
                ForImage = model.Raw.ForImage,
                RawMachineOcrId = model.RawId!.Value,
                Texts = model.Texts.Select(x => new AddressableSpatialText(x.Text, x.Rectangle.ToRectangle()))
            };
        }

        public static ConsolidatedMachineOcrModel ToModel(this ConsolidatedMachineAddressableOcr model)
        {
            return new()
            {
                Consolidation = model.Consolidation,
                Id = model.Id,
                RawId = model.RawMachineOcrId,
                Texts = model.Texts.Select(x => new AddressableSpatialTextModel
                {
                    Created = x.Created,
                    Id = x.Id,
                    Language = model.Language,
                    Rectangle = x.Area.ToModel(),
                    Text = x.Text
                })
            };
        }
        
        public static RawMachineOcr ToBusinessModel(this RawMachineOcrModel model)
        {
            var simpleSpatialTexts = JsonSerializer.Deserialize<SimpleSpatialText[]>(model.Texts.RootElement.GetRawText()) ?? Array.Empty<SimpleSpatialText>();
            
            return new RawMachineOcr
            {
                Id = model.Id,
                Created = model.Created,
                Language = model.Language,
                Provider = model.Provider,
                ForImage = model.ForImage,
                Texts = simpleSpatialTexts.Select(x => new Moe.Ocr.SpatialText(x.Text, x.Area.ToRectangle()))
            };
        }

        private class SimpleSpatialText
        {
            public string Text { get; set; }
            public RectangleModel Area { get; set; }
        }

        public static RawMachineOcrModel ToModel(this RawMachineOcr model)
        {
            var asArray = model.Texts.Select(x => new SimpleSpatialText
            {
                Area = x.Area.ToModel(),
                Text = x.Text
            }).ToArray();
            
            return new RawMachineOcrModel
            {
                Created = model.Created,
                Id = model.Id,
                Language = model.Language,
                Provider = model.Provider,
                ForImage = model.ForImage!.Value,
                Texts = JsonDocument.Parse(JsonSerializer.Serialize(asArray)) 
            };
        }
        
        public static MachineTranslation ToBusinessModel(this MachineTranslationModel model)
        {
            return new()
            {
                Language = model.Texts.First().Language!,
                Provider = model.Provider,
                Texts = model.Texts.Select(x => new BasedOnSpatialText(x.Text, x.Rectangle.ToRectangle())
                {
                    Created = x.Created,
                    Id = x.Id,
                    BasedOn = x.BasedOnSpatialOcrText
                })
            };
        }

        public static MachineTranslationModel ToModel(this MachineTranslation model, int machineId)
        {
            return new ()
            {
                MachineOcrId = machineId,
                Created = DateTime.UtcNow,
                Provider = model.Provider,
                Texts = model.Texts.Select(x => new AddressableSpatialTextModel
                {
                    Created = x.Created,
                    Language = model.Language,
                    Rectangle = x.Area.ToModel(),
                    BasedOnSpatialOcrText = x.BasedOn,
                    Text = x.Text
                })
            };
        }
    }
}