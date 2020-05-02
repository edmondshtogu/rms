using Autofac;
using System;
using System.Linq;
using RMS.Core;
using RMS.Messages.Validation;

namespace RMS.Messages
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds the in memory bus.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddInMemoryBus(this IEngine engine)
        {
            engine.Register(builder =>
            {
                builder.Register(x => new HandlerResolver(engine))
                       .As<IHandlerResolver>()
                       .InstancePerLifetimeScope();

                builder.RegisterType<InMemoryBus>()
                       .As<IBus>()
                       .SingleInstance();
                builder.RegisterType<DefaultCommandValidationProvider>()
                       .As<ICommandValidationProvider>()
                       .SingleInstance();
            });
        }

        /// <summary>
        /// Adds the default decorators.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddDefaultDecorators(this IEngine engine)
        {
            engine.Register(builder =>
            {
                var commandDecorators = engine.FindClassesOfType(typeof(ICommandHandlerDecorator<,>))
                                              .OrderBy(x => GetDecoratorOrder(x))
                                              .ToList();
                if (commandDecorators.Any())
                {
                    commandDecorators.ForEach(decorator =>
                    {
                        builder.RegisterGenericDecorator(decorator, typeof(ICommandHandler<,>));
                    });
                }
            });
        }

        /// <summary>
        /// Adds the handlers.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public static void AddHandlers(this IEngine engine)
        {
            engine.Register(builder =>
            {
                var commandHandlersAsms = engine.FindClassesOfType(typeof(ICommandHandler<,>))
                    .Select(x => x.Assembly)
                    .Distinct()
                    .ToList();
                if (commandHandlersAsms.Any())
                {
                    commandHandlersAsms.ForEach(asm =>
                    {
                        builder.RegisterAssemblyTypes(asm)
                                         .AsClosedTypesOf(typeof(ICommandHandler<,>))
                                         .InstancePerLifetimeScope();
                    });
                }

                var queryHandlersAsms = engine.FindClassesOfType(typeof(IQueryHandler<,>))
                    .Select(x => x.Assembly)
                    .Distinct()
                    .ToList();
                if (queryHandlersAsms.Any())
                {
                    queryHandlersAsms.ForEach(asm =>
                    {
                        builder.RegisterAssemblyTypes(asm)
                            .AsClosedTypesOf(typeof(IQueryHandler<,>))
                            .InstancePerLifetimeScope();
                    });
                }
            });
        }

        private static int GetDecoratorOrder(Type x)
        {
            var decoratorOrder = x.GetCustomAttributes(typeof(DecoratorOrderAttribute), false)
                                   .FirstOrDefault();
            if (decoratorOrder == null)
                return 1000; // No order is specified
            return ((DecoratorOrderAttribute)decoratorOrder).Order;
        }
    }
}