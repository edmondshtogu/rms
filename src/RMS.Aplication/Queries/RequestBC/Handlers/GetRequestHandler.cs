using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Queries.RequestBC.Handlers
{
    public sealed class GetRequestHandler : IQueryHandler<GetRequest, GetRequestResult>
    {
        private readonly IRequestRepository _repository;
        private readonly ITypeAdapter _typeAdapter;

        public GetRequestHandler(IRequestRepository repository, ITypeAdapter typeAdapter)
        {
            _repository = repository;
            _typeAdapter = typeAdapter;
        }

        public async Task<Result<GetRequestResult>> HandleAsync(GetRequest queryModel)
        {
            var result = await Task.Run(() => _repository.GetById(queryModel.Id));

            return Result.Ok(_typeAdapter.Adapt<GetRequestResult>(result.Value));
        }
    }
}
