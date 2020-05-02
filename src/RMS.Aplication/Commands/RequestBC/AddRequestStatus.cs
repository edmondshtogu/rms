using RMS.Messages;
using System;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class AddRequestStatus : ICommand<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
