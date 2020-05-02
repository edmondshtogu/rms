using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RMS.Core
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the App engine.
    /// Edit functionality, modules and implementations access most App functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Gets the container manager.
        /// </summary>
        /// <value>
        /// The container manager.
        /// </value>
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="appFileProvider">File provider</param>
        /// <param name="config">The configuration.</param>
        void Initialize(IAppFileProvider appFileProvider, AppConfig config);

        /// <summary>
        /// Sets the resolver.
        /// </summary>
        void SetResolver();
        
        void Register(Action<ContainerBuilder> registerAction);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve generic type
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(object param, Type type);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve all the concrete implementation of type t
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        IEnumerable<Type> FindClassesOfType<T>(Assembly assembly, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType(Type type, bool onlyConcreteClasses = true);
    }
}
