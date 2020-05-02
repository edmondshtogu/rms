using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RMS.Core.Internal;

namespace RMS.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Represents App engine
    /// </summary>
    public class AppEngine : IEngine
    {

        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        private ContainerManager _containerManager;

        protected AppConfig Configuration { get; private set; }
        protected ContainerBuilder ContainerBuilder { get; private set; }
        protected ITypeFinder TypeFinder { get; private set; }
        protected IAppFileProvider FileProvider { get; private set; }

        /// <summary>
        /// Service provider
        /// </summary>
        public virtual ContainerManager ContainerManager => _containerManager;

        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="appFileProvider">The application file provider.</param>
        /// <param name="config">The configuration.</param>
        public void Initialize(IAppFileProvider appFileProvider, AppConfig config)
        {
            Configuration = config;
            ContainerBuilder = new ContainerBuilder();
            FileProvider = appFileProvider;
            TypeFinder = new AppTypeFinder(appFileProvider);

            //register the instances to be available where are required
            ContainerBuilder.RegisterInstance(config).As<AppConfig>().SingleInstance();
            ContainerBuilder.RegisterInstance(TypeFinder).As<ITypeFinder>().SingleInstance();
            ContainerBuilder.RegisterInstance(appFileProvider).As<IAppFileProvider>().SingleInstance();

            ConfigureAppEngine();
        }

        /// <summary>
        /// Sets the resolver.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void SetResolver()
        {
            // nothing here
        }

        /// <summary>
        /// Registers the specified register action.
        /// </summary>
        /// <param name="registerAction">The register action.</param>
        public void Register(Action<ContainerBuilder> registerAction)
        {
            registerAction(ContainerBuilder);
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
        {
            return (T)ContainerManager.Resolve(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>
        /// Resolved service
        /// </returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        /// <summary>
        /// Resolve generic type
        /// </summary>
        /// <param name="param"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(object param, Type type)
        {
            var paramType = param.GetType();
            var genericType = type.MakeGenericType(paramType);
            return ContainerManager.Resolve(genericType);
        }

        /// <summary>
        /// Resolve all the concrete implementation of type t
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        /// <summary>
        /// Finds the type of the classes of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onlyConcreteClasses">if set to <c>true</c> [only concrete classes].</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return TypeFinder.FindClassesOfType<T>(onlyConcreteClasses);
        }

        /// <summary>
        /// Finds the type of the classes of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="onlyConcreteClasses">if set to <c>true</c> [only concrete classes].</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(Assembly assembly, bool onlyConcreteClasses = true)
        {
            return TypeFinder.FindClassesOfType<T>(new List<Assembly> { assembly }, onlyConcreteClasses);
        }

        /// <summary>
        /// Finds the type of the classes of.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="onlyConcreteClasses">if set to <c>true</c> [only concrete classes].</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType(Type type, bool onlyConcreteClasses = true)
        {
            return TypeFinder.FindClassesOfType(type, onlyConcreteClasses);
        }

        /// <summary>
        /// create an instance per each Startup class that implements IAppStartup and then call ConfigureContainerBuilder
        /// </summary>
        protected virtual void ConfigureAppEngine()
        {
            var startupConfigurations = TypeFinder.FindClassesOfType<IAppStartup>();
            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Select(startup => (IAppStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure services
            foreach (var instance in instances)
            {
                instance.ConfigureAppEngine(this, TypeFinder, Configuration);
            }
        }

        /// <summary>
        /// Sets the container manager.
        /// </summary>
        /// <param name="containerManager">The container manager.</param>
        protected void SetContainerManager(ContainerManager containerManager)
        {
            _containerManager = containerManager;
        }
    }
}
