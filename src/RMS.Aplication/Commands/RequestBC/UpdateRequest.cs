using RMS.Messages;
using System;
using System.ComponentModel.DataAnnotations;

namespace RMS.Application.Commands.RequestBC
{
    public sealed class UpdateRequest : ICommand<bool>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime RaisedDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        public Guid StatusId { get; set; }

        public string StatusName { get; private set; }

        public string StatusDescription { get; private set; }

        public string Attachments { get; private set; }
    }
}
