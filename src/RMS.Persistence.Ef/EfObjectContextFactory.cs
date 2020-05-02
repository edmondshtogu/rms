using RMS.Core;
using RMS.Core.Infrastructure;
using System;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace RMS.Persistence.Ef
{
    /// <summary>
    /// Represents base object context
    /// </summary>
    public class EfObjectContextFactory : IDbContextFactory<EfObjectContext>
    {
        private readonly AppConfig _appConfig;

        public EfObjectContextFactory()
        {
            _appConfig = ConfigurationManager.GetSection("appConfig") as AppConfig;

            var fileProvider = new AppFileProvider(AppContext.BaseDirectory);

            EngineContext.Create<AppEngine>().Initialize(fileProvider, _appConfig);
        }

        public EfObjectContext Create()
        {
            return new EfObjectContext(_appConfig.DataConnectionString);
        }
    }
}