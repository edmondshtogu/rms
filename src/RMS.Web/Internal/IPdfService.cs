using RMS.Application.Queries.RequestBC;

namespace RequestsManagementSystem.Internal
{
    public interface IPdfService
    {
        byte[] GeneratePdfReportForRequest(GetRequestResult request);
    }
}