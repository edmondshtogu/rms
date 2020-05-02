using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class AddRequestStatusHandler : ICommandHandler<AddRequestStatus, Guid>
    {
        private readonly IRequestStatusRepository _repository;

        public AddRequestStatusHandler(IRequestStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> HandleAsync(AddRequestStatus command)
        {
            var result = RequestStatus.Create(Guid.NewGuid(),
                                        command.Name,
                                        command.Description);
            if (result.IsFailure)
            {
                return Result.Fail<Guid>(result.Error);
            }
            await Task.Run(() => _repository.Create(result.Value));

            return Result.Ok(result.Value.Id);
        }
    }
}
