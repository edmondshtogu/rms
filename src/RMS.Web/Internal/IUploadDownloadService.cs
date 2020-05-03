using System;
using System.Threading.Tasks;
using System.Web;

namespace RequestsManagementSystem.Internal
{
    public interface IUploadDownloadService
    {
        Task<string> UploadNewAttachmentAsync(Guid ownerId, HttpPostedFileBase postedFileBase);

        Task CleanAttachmentsOwnedByAsync(Guid ownerId);

        Task<(byte[] fileContents, string contentType, string fileName)> DownloadAttachmentsOwnedByAsync(Guid ownerId);
    }
}