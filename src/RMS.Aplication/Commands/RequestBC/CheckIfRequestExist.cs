using RMS.Messages;
using System;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class CheckIfRequestExist : ICommand<bool>
    {
        public CheckIfRequestExist()
        {
        }

        public CheckIfRequestExist(string name, DateTime reaisedDate)
        {
            Name = name;
            RaisedDate = reaisedDate;
        }

        public string Name { get; set; }
        public DateTime RaisedDate { get; set; }
    }
}
