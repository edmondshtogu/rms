using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMS.Application.Queries.RequestBC.Handlers
{
    public sealed class GetRequestStatusesHandler : IQueryHandler<GetRequestStatuses, IEnumerable<GetRequestStatusResult>>
    {
        private readonly IRequestStatusRepository _repository;
        private readonly ITypeAdapter _typeAdapter;

        public GetRequestStatusesHandler(IRequestStatusRepository repository, ITypeAdapter typeAdapter)
        {
            _repository = repository;
            _typeAdapter = typeAdapter;
        }

        public async Task<Result<IEnumerable<GetRequestStatusResult>>> HandleAsync(GetRequestStatuses queryModel)
        {
            var data = _repository.GetAll();

            var result = await Task.Run(
                () => _typeAdapter.Adapt<IEnumerable<GetRequestStatusResult>>(data)
            );

            return Result.Ok(result);
        }
    }
}
