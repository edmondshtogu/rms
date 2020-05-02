namespace RMS.Core
{
    /// <summary>
    /// Represents object for the configuring services and middleware on application startup
    /// </summary>
    public interface IAppStartup
    {
        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="configuration">Configuration root of the application</param>
        void ConfigureAppEngine(IEngine engine, ITypeFinder typeFinder, AppConfig configuration);
    }
}
