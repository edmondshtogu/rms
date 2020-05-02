using Autofac;
using RMS.Core.Domain;
using RMS.Core.Extensions;
using RMS.Persistence.Ef;
using RMS.Persistence.Ef.DataProviders;
using System;
using System.Linq;

namespace RMS.Core
{
    public static class EngineExtensions
    {
        /// <summary>
        /// Adds entity framework services
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="configuration"></param>
        public static void AddDbServices(this IEngine engine, AppConfig configuration)
        {
            engine.Register(builder =>
            {
                builder.Register(x => GetDataProvider(configuration))
                       .As<IDataProvider>()
                       .InstancePerDependency();
                builder.Register<IDbContext>(c => new EfObjectContext(configuration.DataConnectionString)).InstancePerLifetimeScope();
            
                var assembliesWithRepositories = engine.FindClassesOfType(typeof(IRepository<>))
                                                        .Select(x => x.Assembly)
                                                        .DistinctBy(x => x.FullName)
                                                        .ToList();

                foreach (var asm in assembliesWithRepositories)
                {
                    builder.RegisterAssemblyTypes(asm)
                            .AsClosedTypesOf(typeof(IRepository<>))
                            .InstancePerLifetimeScope();
                }
            });
        }

        public static void AddDefaultPagination(this IEngine engine, Action<PaginationOptions> option)
        {
            var paginationOptions = new PaginationOptions();
            option.Invoke(paginationOptions);

            engine.Register(builder =>
            {
                builder.Register(c => new PaginationService(c.Resolve<ITypeAdapter>(), paginationOptions))
                       .As<IPaginationService>()
                       .SingleInstance();
            });
        }

        private static IDataProvider GetDataProvider(AppConfig configuration)
        {

            var providerName = configuration.DataProvider.ToLowerInvariant();

            switch (providerName)
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                default:
                    return new SqlCeDataProvider();
            }
        }
    }
}
