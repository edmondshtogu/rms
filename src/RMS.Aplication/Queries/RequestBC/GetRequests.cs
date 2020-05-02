using RMS.Core;
using RMS.Messages;

namespace RMS.Application.Queries.RequestBC
{
    public sealed class GetRequests : IQuery<PaginatedItemsResult<GetRequestResult>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
