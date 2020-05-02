using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class UpdateRequestStatusHandler : ICommandHandler<UpdateRequestStatus, bool>
    {
        private readonly IRequestStatusRepository _repository;

        public UpdateRequestStatusHandler(IRequestStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> HandleAsync(UpdateRequestStatus command)
        {
            var resultOrNothing = await Task.Run(() => _repository.GetById(command.Id));
            if (resultOrNothing.HasNoValue)
            {
                return Result.Fail<bool>(ErrorMessages.RequestNotFound);
            }
            var result = resultOrNothing.Value.UpdateDetails(command.Name, command.Description);
            if (result.IsFailure)
            {
                return Result.Fail<bool>(result.Error);
            }
            await Task.Run(() => _repository.Update(resultOrNothing.Value));
            return Result.Ok(true);
        }
    }
}
