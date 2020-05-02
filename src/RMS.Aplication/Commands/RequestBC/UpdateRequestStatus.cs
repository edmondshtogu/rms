using RMS.Messages;
using System;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class UpdateRequestStatus : ICommand<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
