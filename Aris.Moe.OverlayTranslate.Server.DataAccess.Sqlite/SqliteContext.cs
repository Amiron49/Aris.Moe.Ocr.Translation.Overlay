using System;
using System.Text.Json;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite
{
    //dotnet ef migrations add --startup-project Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite --project Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite
    public class SqliteContext : OverlayTranslateServerContext
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public SqliteContext(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
            if (_databaseConfiguration.ConnectionString == null)
                // ReSharper disable once CA2208
                throw new ArgumentNullException(nameof(databaseConfiguration.ConnectionString));
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_databaseConfiguration.ConnectionString!);

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
    
    // ReSharper disable once UnusedType.Global
    public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<SqliteContext>
    {
        public SqliteContext CreateDbContext(string[] args)
        {
            return new SqliteContext(new DatabaseConfiguration
            {
                ConnectionString = ":memory:",
            });
        }
    }
}