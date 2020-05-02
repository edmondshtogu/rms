using RMS.Messages;
using System;

namespace RMS.Application.Queries.RequestBC
{
    public sealed class GetRequestStatus : IQuery<GetRequestStatusResult>
    {
        public Guid Id { get; set; }
    }

    public sealed class GetRequestStatusResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
