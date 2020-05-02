using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class AddRequestHandler : ICommandHandler<AddRequest, Guid>
    {
        private readonly IRequestRepository _repository;

        public AddRequestHandler(IRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> HandleAsync(AddRequest command)
        {
            var result = Request.Create(Guid.NewGuid(),
                                        command.Name,
                                        command.Description,
                                        command.RaisedDate,
                                        command.DueDate,
                                        command.StatusId,
                                        command.Attachments);
            if (result.IsFailure)
            {
                return Result.Fail<Guid>(result.Error);
            }
            await Task.Run(() => _repository.Create(result.Value));

            return Result.Ok(result.Value.Id);
        }
    }
}
