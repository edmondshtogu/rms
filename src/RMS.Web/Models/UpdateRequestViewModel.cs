using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using System.Collections.Generic;
using System.Web;

namespace RequestsManagementSystem.Models
{
    public class UpdateRequestViewModel
    {
        public UpdateRequest UpdateRequestCommand { get; set; }
        public int SelectedRequestStatus { get; set; }
        public IEnumerable<GetRequestStatusResult> RequestStatuses { get; set; }
        public HttpPostedFileBase AttachmentFile { get; set; }
    }
}