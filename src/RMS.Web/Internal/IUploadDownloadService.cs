using System;
using System.Web;

namespace RequestsManagementSystem.Internal
{
    public interface IUploadDownloadService
    {
        string UploadNewAttachment(Guid ownerId, HttpPostedFileBase postedFileBase);

        void CleanAttachmentsOwnedBy(Guid ownerId);
    }
}