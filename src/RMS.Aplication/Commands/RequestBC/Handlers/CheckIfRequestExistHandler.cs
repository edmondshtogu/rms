using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Application.Commands.RequestBC.Handlers
{
    public sealed class CheckIfRequestExistHandler : ICommandHandler<CheckIfRequestExist, bool>
    {
        private readonly IRequestRepository _repository;

        public CheckIfRequestExistHandler(IRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> HandleAsync(CheckIfRequestExist command)
        {
            var raisedAt = command.RaisedDate.ToString("yyyyMMdd");
            var result = await Task.Run(() => 
                _repository.AsNoTracking()
                    .Where(request => request.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .Any(request => request.RaisedDate.ToString("yyyyMMdd").Equals(raisedAt))
            );

            return Result.Ok(result);
        }
    }
}
