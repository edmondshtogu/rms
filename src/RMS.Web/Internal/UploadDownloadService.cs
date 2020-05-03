using RMS.Core;
using System;
using System.Web;

namespace RequestsManagementSystem.Internal
{
    public class UploadDownloadService : IUploadDownloadService
    {
        private readonly AppConfig _appConfig;
        private readonly IAppFileProvider _fileProvider;

        public UploadDownloadService(IAppFileProvider fileProvider, AppConfig appConfig)
        {
            _fileProvider = fileProvider;
            _appConfig = appConfig;
        }

        public void CleanAttachmentsOwnedBy(Guid ownerId)
        {
            var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);

            storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

            _fileProvider.DeleteDirectory(storagePath);
        }

        public string UploadNewAttachment(Guid ownerId, HttpPostedFileBase postedFileBase)
        {
            //Use Namespace called :  System.IO  
            var fileName = _fileProvider.GetFileNameWithoutExtension(postedFileBase.FileName);

            //To Get File Extension  
            var fileExtension = _fileProvider.GetFileExtension(postedFileBase.FileName);

            //Add Current Date To Attached File Name  
            fileName = $"{DateTime.Now:yyyyMMddhhmmss}-{fileName.Trim()}{fileExtension}";

            var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);

            storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

            _fileProvider.CreateDirectory(storagePath);

            storagePath = _fileProvider.Combine(storagePath, fileName);

            //To copy and save file into server.  
            postedFileBase.SaveAs(storagePath);

            return _fileProvider.GetFileName(postedFileBase.FileName);
        }
    }
}