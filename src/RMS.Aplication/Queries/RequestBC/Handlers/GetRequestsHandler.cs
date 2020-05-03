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
            var columnFilters = queryModel.ColumnFilters;
            var search = queryModel.SearchString?.Trim();

            var result = await Task.Run(() => _paginationService.Paginate<GetRequestResult, Request>
                                (
                                    baseQuery,
                                    queryModel.OrderByString,
                                    queryModel.PageIndex,
                                    queryModel.PageSize,
                                    filter
                                ));

            return Result.Ok(result);

            bool filter(Request r)
            {
                var containsSearch = !(
                    search != null 
                    && (r.Name == null || !r.Name.ToLower().Contains(search.ToLower())) 
                    && (r.Description == null || !r.Description.ToLower().Contains(search.ToLower())) 
                    && (r.RaisedDate.ToString("dd-MM-yyyy hh:mm") == null 
                        || !r.RaisedDate.ToString("dd-MM-yyyy hh:mm").ToLower().Contains(search.ToLower())) 
                    && (r.DueDate.ToString("dd-MM-yyyy hh:mm") == null 
                        || !r.DueDate.ToString("dd-MM-yyyy hh:mm").ToLower().Contains(search.ToLower()))
               );

                var containsColFilter = 
                    (columnFilters[0] == null || 
                     (r.Name != null && r.Name.ToLower().Contains(columnFilters[0].ToLower())))
                 && (columnFilters[1] == null || 
                     (r.Description != null && r.Description.ToLower().Contains(columnFilters[1].ToLower())))
                 && (columnFilters[2] == null || 
                     (r.RaisedDate.ToString("dd-MM-yyyy hh:mm") != null 
                      && r.RaisedDate.ToString("dd-MM-yyyy hh:mm").ToLower().Contains(columnFilters[2].ToLower())))
                 && (columnFilters[3] == null || 
                     (r.DueDate.ToString("dd-MM-yyyy hh:mm") != null 
                      && r.DueDate.ToString("dd-MM-yyyy hh:mm").ToLower().Contains(columnFilters[3].ToLower())));

                return containsSearch && containsColFilter;
            }            
        }
    }
}
