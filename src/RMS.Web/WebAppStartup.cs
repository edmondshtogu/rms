using RequestsManagementSystem.Extentions;
using RMS.Core;
using RMS.Messages;

namespace RequestsManagementSystem
{
    /// <summary>
    /// Execution Proxy Service Startup
    /// </summary>
    /// <seealso cref="RMS.Core.IAppStartup" />
    public class WebAppStartup : IAppStartup
    {
        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1;

        public void ConfigureAppEngine(IEngine engine, ITypeFinder typeFinder, AppConfig configuration)
        {
            engine.AddWebAppBaseInfrastructure();
            engine.AddAutoMapper();
            engine.AddDbServices(configuration);
            engine.AddInMemoryBus();
            engine.AddFluentValidation();
            engine.AddHandlers();
            engine.AddDefaultDecorators();
            engine.AddDefaultPagination(c =>
            {
                c.MaxPageSizeAllowed = 100;
            });
        }
    }
}