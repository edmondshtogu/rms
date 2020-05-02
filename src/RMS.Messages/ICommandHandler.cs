using System.Threading.Tasks;
using RMS.Core.Domain;

namespace RMS.Messages
{
    public interface ICommandHandler<in TCommand, TCommandResult>
        where TCommand : ICommand<TCommandResult>
    {
        Task<Result<TCommandResult>> HandleAsync(TCommand command);
    }
}