using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class DeleteRequestStatusHandler : ICommandHandler<DeleteRequestStatus, bool>
    {
        private readonly IRequestStatusRepository _repository;

        public DeleteRequestStatusHandler(IRequestStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> HandleAsync(DeleteRequestStatus command)
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
