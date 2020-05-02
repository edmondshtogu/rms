using RMS.Messages;
using System;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class DeleteRequest : ICommand<bool>
    {
        public DeleteRequest()
        {
        }

        public DeleteRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
