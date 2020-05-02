using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Queries.RequestBC.Handlers
{
    public sealed class GetRequestsHandler : IQueryHandler<GetRequests, PaginatedItemsResult<GetRequestResult>>
    {
        private readonly IRequestRepository _repository;
        private readonly IPaginationService _paginationService;

        public GetRequestsHandler(IRequestRepository repository,
                                  IPaginationService paginationService)
        {
            _repository = repository;
            _paginationService = paginationService;
        }

        public async Task<Result<PaginatedItemsResult<GetRequestResult>>> HandleAsync(GetRequests queryModel)
        {
            var baseQuery = _repository.AsNoTracking();

            var result = await Task.Run(() => _paginationService.Paginate<GetRequestResult, Request, string>
                                (
                                    baseQuery,
                                    item => item.Name,
                                    queryModel.PageIndex,
                                    queryModel.PageSize
                                ));

            return Result.Ok(result);
        }
    }
}
