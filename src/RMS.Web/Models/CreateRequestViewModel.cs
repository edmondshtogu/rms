using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using System.Collections.Generic;
using System.Web;

namespace RequestsManagementSystem.Models
{
    public class CreateRequestViewModel
    {
        public AddRequest AddRequestCommand { get; set; }
        public IEnumerable<GetRequestStatusResult> RequestStatuses { get; set; }
        public HttpPostedFileBase AttachmentFile { get; set; }
    }
}