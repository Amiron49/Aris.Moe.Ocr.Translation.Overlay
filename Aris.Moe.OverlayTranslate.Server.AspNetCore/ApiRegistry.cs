using System.Threading.Tasks;
using Aris.Moe.OverlayTranslate.Configuration;
using Aris.Moe.OverlayTranslate.Server.DataAccess;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Model;
using Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite;
using Aris.Moe.OverlayTranslate.Server.Image;
using Aris.Moe.OverlayTranslate.Server.Ocr;
using Aris.Moe.OverlayTranslate.Server.Translation;
using Aris.Moe.Translate;
using FluentResults;
using Lamar;
using Microsoft.EntityFrameworkCore;

namespace Aris.Moe.OverlayTranslate.Server.AspNetCore
{
    
    public class ApiRegistry : ServiceRegistry
    {
        public ApiRegistry(ApiConfiguration apiConfiguration)
        {
            For<ITranslateConfig>().Use(apiConfiguration);
            For<DatabaseConfiguration>().Use(apiConfiguration.Database);
            IncludeRegistry(new ServerRegistry(apiConfiguration));
            IncludeRegistry<DataAccessRegistry>();
            For<IResultLogger>().Use<ResultLogger>();
        }
    }

    public class DataAccessRegistry : ServiceRegistry
    {
        public DataAccessRegistry()
        {
            For<IImageReferenceRepository>().Use<ImageReferenceRepository>();
            For<IMachineOcrRepository>().Use<MachineOcrRepository>();
            For<ITranslationRepository>().Use<TranslationRepository>();
            For<OverlayTranslateServerContext>().Use<SqliteContext>();

            For<IOnStartup>().Add<MigrateDataBase>();
        }
    }
    
    public class MigrateDataBase : IOnStartup
    {
        private readonly OverlayTranslateServerContext _context;

        public MigrateDataBase(OverlayTranslateServerContext context)
        {
            _context = context;
        }
        
        public Task OnStartup()
        {
            return _context.Database.MigrateAsync();
        }
    }
}