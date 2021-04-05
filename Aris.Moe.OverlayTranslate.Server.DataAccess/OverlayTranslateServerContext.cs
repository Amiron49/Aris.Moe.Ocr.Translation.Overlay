using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess
{
    public abstract class OverlayTranslateServerContext : DbContext
    {
        public DbSet<ImageReferenceModel> Images { get; set; } = null!;
        public DbSet<RawMachineOcrModel> RawMachineOcrs { get; set; } = null!;
        public DbSet<ConsolidatedMachineOcrModel> ConsolidatedMachineOcrs { get; set; } = null!;
        public DbSet<AddressableSpatialTextModel> SpatialTexts { get; set; } = null!;
        public DbSet<MachineTranslationModel> MachineTranslations { get; set; } = null!;
        public DbSet<VoteModel> Votes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageReferenceModel>(reference =>
            {
                reference.OwnsOne(x => x.Info, builder =>
                {
                    builder.HasIndex(x => x.Sha256Hash);
                    builder.HasIndex(x => x.AverageHash);
                });
            });
            modelBuilder.Entity<RawMachineOcrModel>(rawMachineOcr =>
            {
                rawMachineOcr.HasOne<ImageReferenceModel>().WithMany().HasForeignKey(x => x.ForImage);
            });
            modelBuilder.Entity<ConsolidatedMachineOcrModel>(consolidatedMachineOcr =>
            {
                consolidatedMachineOcr.HasOne(x => x.Raw).WithMany().HasForeignKey(x => x.RawId);
                consolidatedMachineOcr.HasMany(x=> x.Texts).WithOne(x => x.MachineOcr!).HasForeignKey(x => x.MachineOcrId);
            });

            modelBuilder.Entity<AddressableSpatialTextModel>(addressableSpatial =>
            {
                addressableSpatial.HasMany<AddressableSpatialTextModel>().WithOne().HasForeignKey(x => x.BasedOnSpatialOcrText);
                addressableSpatial.HasMany<VoteModel>().WithOne().HasForeignKey(x => x.For);
                addressableSpatial.OwnsOne(x => x.Rectangle, builder =>
                {
                    builder.OwnsOne(x => x.BottomRight);
                    builder.OwnsOne(x => x.TopLeft);
                });
            });

            modelBuilder.Entity<MachineTranslationModel>(translation =>
            {
                translation.HasOne(x => x.MachineOcr).WithMany().HasForeignKey(x => x.MachineOcrId);
                translation.HasMany(x => x.Texts).WithOne().HasForeignKey(x => x.MachineTranslationId);
            });

            modelBuilder.Entity<VoteModel>(vote =>
            {
                vote.HasKey(x => new {x.For, x.UserId});
                vote.Property(x => x.ChangedOn).ValueGeneratedOnAddOrUpdate();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}