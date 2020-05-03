using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using FluentValidation;
using RequestsManagementSystem.Internal;
using RequestsManagementSystem.Mapping;
using RequestsManagementSystem.Validations;
using RMS.Core;
using RMS.Messages.Validation;
using System;
using System.Linq;

namespace RequestsManagementSystem.Extentions
{
    /// <summary>
    /// Register application dependencies
    /// </summary>

    public static class EngineExtensions
    {
        /// <summary>
        /// Adds the web application base infrastructure.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddWebAppBaseInfrastructure(this IEngine engine)
        {
            engine.Register(container =>
            {
                // Register your MVC controllers. (MvcApplication is the name of
                // the class in Global.asax.)
                container.RegisterControllers(typeof(MvcApplication).Assembly);

                // OPTIONAL: Register model binders that require DI.
                container.RegisterModelBinders(typeof(MvcApplication).Assembly);
                container.RegisterModelBinderProvider();

                // OPTIONAL: Register web abstractions like HttpContextBase.
                container.RegisterModule<AutofacWebTypesModule>();

                // OPTIONAL: Enable property injection in view pages.
                container.RegisterSource(new ViewRegistrationSource());

                // OPTIONAL: Enable property injection into action filters.
                container.RegisterFilterProvider();
            });
        }        

        /// <summary>
        /// Adds the fluent validation.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddFluentValidation(this IEngine engine)
        {
            engine.Register(builder =>
            {
                var asmsWithCommandValidator = engine.FindClassesOfType(typeof(IValidator<>))
                                                    .Select(x => x.Assembly)
                                                    .Distinct()
                                                    .ToList();

                foreach (var asm in asmsWithCommandValidator)
                {
                    builder.RegisterAssemblyTypes(asm)
                            .AsClosedTypesOf(typeof(IValidator<>))
                            .InstancePerLifetimeScope();
                }
                builder.RegisterType<FluentValidationProvider>()
                       .As<ICommandValidationProvider>()
                       .InstancePerLifetimeScope();
            });
        }

        /// <summary>
        /// Adds the automatic mapper.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddAutoMapper(this IEngine engine)
        {
            //find mapper configurations provided by other assemblies
            var mapperConfigurations = engine.FindClassesOfType<IMapperProfile>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            engine.Register(builder =>
            {
                builder.RegisterInstance(config.CreateMapper()).As<IMapper>().SingleInstance();
                builder.RegisterType<AutoMapperTypeAdapter>().As<ITypeAdapter>().SingleInstance();
            });
        }

        public static void AddUploadDownloadService(this IEngine engine)
        {
            engine.Register(builder =>
            {
                builder.RegisterType<UploadDownloadService>().As<IUploadDownloadService>().InstancePerLifetimeScope();
            });
        }
    }
}