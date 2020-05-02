using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class UpdateRequestHandler : ICommandHandler<UpdateRequest, bool>
    {
        private readonly IRequestRepository _repository;

        public UpdateRequestHandler(IRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> HandleAsync(UpdateRequest command)
        {
            var resultOrNothing = await Task.Run(() => _repository.GetById(command.Id));
            if (resultOrNothing.HasNoValue)
            {
                return Result.Fail<bool>(ErrorMessages.RequestNotFound);
            }

            var result = resultOrNothing.Value.UpdateDetails(command.Name,
                                        command.Description,
                                        command.RaisedDate,
                                        command.DueDate,
                                        command.StatusId,
                                        command.Attachments);
            if (result.IsFailure)
            {
                return Result.Fail<bool>(result.Error);
            }
            await Task.Run(() => _repository.Update(resultOrNothing.Value));
            return Result.Ok(true);
        }
    }
}
