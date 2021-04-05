using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Server.SpatialText;
using Aris.Moe.OverlayTranslate.Server.Translation;
using Aris.Moe.OverlayTranslate.Server.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Model
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly OverlayTranslateServerContext _context;

        public TranslationRepository(OverlayTranslateServerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MachineTranslation>> GetMachineTranslations(Guid imageId)
        {
            var translations = await _context.MachineTranslations.Where(x => x.MachineOcr.Raw.ForImage == imageId)
                .Include(x => x.Texts).ToListAsync();

            return translations.Select(translationModel =>
            {
                var texts = translationModel.Texts.Select(x =>
                    new BasedOnSpatialText(x.Text, x.Rectangle.ToRectangle())
                    {
                        Created = x.Created,
                        Id = x.Id,
                        BasedOn = x.BasedOnSpatialOcrText
                    });
                
                return new MachineTranslation(
                    translationModel.Texts.First().Language!,
                    texts,
                    translationModel.Provider
                );
            });
        }

        public async Task<MachineTranslation> SaveMachineTranslation(int machineId, MachineTranslation machineTranslation)
        {
            // ReSharper disable once MethodHasAsyncOverload
            var result = _context.MachineTranslations.Add(machineTranslation.ToModel(machineId));
            await _context.SaveChangesAsync();
            return result.Entity.ToBusinessModel();
        }
    }
}