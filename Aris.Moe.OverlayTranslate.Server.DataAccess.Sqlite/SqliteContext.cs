using System;
using System.Text.Json;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite
{
    //dotnet ef migrations add --startup-project Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite --project Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite
    public class SqliteContext : OverlayTranslateServerContext
    {
        private readonly ISqliteConnectionManager _sqliteConnectionManager;

        public SqliteContext(ISqliteConnectionManager sqliteConnectionManager)
        {
            _sqliteConnectionManager = sqliteConnectionManager;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_sqliteConnectionManager.Connection);

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
            return new(new InMemorySqliteConnectionManager());
        }
    }

    public interface ISqliteConnectionManager : IDisposable
    {
        SqliteConnection Connection { get; }
    }

    public class InMemorySqliteConnectionManager : ISqliteConnectionManager
    {
        public SqliteConnection Connection => new("Data Source=file:lel?mode=memory&cache=shared");
        
        private readonly SqliteConnection _keepAliveConnection;
        
        public InMemorySqliteConnectionManager()
        {
            _keepAliveConnection = Connection;
            _keepAliveConnection.Open();
        }
        
        public void Dispose()
        {
            _keepAliveConnection.Dispose();
        }
    }
}