using RMS.Core;
using RMS.Core.Domain;
using RMS.Core.Domain.RequestBC;
using RMS.Messages;
using System.Threading.Tasks;

namespace RMS.Application.Queries.RequestBC.Handlers
{
    public sealed class GetRequestStatusesHandler : IQueryHandler<GetRequestStatuses, PaginatedItemsResult<GetRequestStatusResult>>
    {
        private readonly IRequestStatusRepository _repository;
        private readonly IPaginationService _paginationService;

        public GetRequestStatusesHandler(IRequestStatusRepository repository,
                                  IPaginationService paginationService)
        {
            _repository = repository;
            _paginationService = paginationService;
        }

        public async Task<Result<PaginatedItemsResult<GetRequestStatusResult>>> HandleAsync(GetRequestStatuses queryModel)
        {
            var baseQuery = _repository.AsNoTracking();

            var result = await Task.Run(() => _paginationService.Paginate<GetRequestStatusResult, RequestStatus>
                                (
                                    baseQuery,
                                    "Name asc",
                                    queryModel.PageIndex,
                                    queryModel.PageSize
                                ));

            return Result.Ok(result);
        }
    }
}
