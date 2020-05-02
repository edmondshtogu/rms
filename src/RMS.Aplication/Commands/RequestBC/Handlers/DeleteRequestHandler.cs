using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class DeleteRequestHandler : ICommandHandler<DeleteRequest, bool>
    {
        private readonly IRequestRepository _repository;

        public DeleteRequestHandler(IRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> HandleAsync(DeleteRequest command)
        {
            var resultOrNothing = await Task.Run(() => _repository.GetById(command.Id));
            if (resultOrNothing.HasNoValue)
            {
                return Result.Ok(true);
            }
            await Task.Run(() => _repository.Delete(resultOrNothing.Value));
            return Result.Ok(true);
        }
    }
}
