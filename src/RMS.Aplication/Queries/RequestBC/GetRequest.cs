using RMS.Messages;
using System;
using System.ComponentModel.DataAnnotations;

namespace RMS.Application.Queries.RequestBC
{
    public sealed class GetRequest : IQuery<GetRequestResult>
    {
        public Guid Id { get; set; }
    }

    public sealed class GetRequestResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Raised Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        [DataType(DataType.DateTime), Required]
        public DateTime RaisedDate { get; set; }

        [Display(Name = "Due Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        [DataType(DataType.DateTime), Required]
        public DateTime DueDate { get; set; }

        [Display(Name = "Created Date (UTC)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        [DataType(DataType.DateTime), Required]
        public DateTime CreatedOnUtc { get; set; }

        [Display(Name = "Updated Date (UTC)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        [DataType(DataType.DateTime), Required]
        public DateTime UpdatedOnUtc { get; set; }

        public Guid StatusId { get; set; }

        [Display(Name = "Setatus")]
        public string StatusName { get; set; }

        public string StatusDescription { get; set; }

        public string Attachments { get; set; }
    }
}
