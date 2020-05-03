using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using System.Collections.Generic;

namespace RequestsManagementSystem.Models
{
    public class UpdateRequestViewModel
    {
        public UpdateRequest UpdateRequestCommand { get; set; }
        public int SelectedRequestStatus { get; set; }
        public IEnumerable<GetRequestStatusResult> RequestStatuses { get; set; }
    }
}