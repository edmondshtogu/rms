using RMS.Core;
using RMS.Messages;

namespace RMS.Application.Queries.RequestBC
{
    public sealed class GetRequestStatuses : IQuery<PaginatedItemsResult<GetRequestStatusResult>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
