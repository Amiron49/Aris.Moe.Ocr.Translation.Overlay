using System.Text.Json;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite
{
    public class SqliteContext : OverlayTranslateServerContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("W:\\temp.sqlite");
            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawMachineOcrModel>(rawMachineOcr =>
            {
                rawMachineOcr
                    .Property(x => x.Texts)
                    .HasConversion(document => document.RootElement.GetRawText(), s => JsonDocument.Parse(s, default));
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}