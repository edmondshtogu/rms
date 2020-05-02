using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Queries.RequestBC.Handlers
{
    public sealed class GetRequestStatusHandler : IQueryHandler<GetRequestStatus, GetRequestStatusResult>
    {
        private readonly IRequestStatusRepository _repository;
        private readonly ITypeAdapter _typeAdapter;

        public GetRequestStatusHandler(IRequestStatusRepository repository, ITypeAdapter typeAdapter)
        {
            _repository = repository;
            _typeAdapter = typeAdapter;
        }

        public async Task<Result<GetRequestStatusResult>> HandleAsync(GetRequestStatus queryModel)
        {
            var result = await Task.Run(() => _repository.GetById(queryModel.Id));

            return Result.Ok(_typeAdapter.Adapt<GetRequestStatusResult>(result.Value));
        }
    }
}
