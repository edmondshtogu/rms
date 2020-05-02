using RMS.Messages;
using System;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class DeleteRequestStatus : ICommand<bool>
    {
        public DeleteRequestStatus()
        {
        }

        public DeleteRequestStatus(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
