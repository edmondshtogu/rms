using System;

namespace RMS.Messages
{
    public interface IHandlerResolver
    {
        object ResolveHandler(Type handlerType);

        object ResolveCommandHandler(object param, Type type);

        object ResolveQueryHandler(object query, Type type);
    }
}