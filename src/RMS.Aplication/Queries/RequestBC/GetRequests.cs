using RMS.Core;
using RMS.Messages;

namespace RMS.Application.Queries.RequestBC
{
    public sealed class GetRequests : IQuery<PaginatedItemsResult<GetRequestResult>>
    {
        public string OrderByString { get; set; }
        public string SearchString { get; set; }
        public string[] ColumnFilters { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
