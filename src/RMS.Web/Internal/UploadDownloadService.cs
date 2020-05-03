using RMS.Core;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
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

        public async Task CleanAttachmentsOwnedByAsync(Guid ownerId)
        {
            await Task.Run(() =>
            {
                var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);

                storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

                _fileProvider.DeleteDirectory(storagePath);
            });
        }

        public async Task<string> UploadNewAttachmentAsync(Guid ownerId, HttpPostedFileBase postedFileBase)
        {
            return await Task.Run(() =>
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
            });
        }

        public async Task<(byte[] fileContents, string contentType, string fileName)> DownloadAttachmentsOwnedByAsync(Guid ownerId)
        {
            return await Task.Run(() =>
            {
                var fileName = "Attachments.zip";

                var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);

                storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

                _fileProvider.CreateDirectory(storagePath);

                storagePath = _fileProvider.Combine(storagePath, fileName);

                //////int CurrentFileID = Convert.ToInt32(FileID);  
                var filesCol = _fileProvider.GetFiles(storagePath);
                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        for (int i = 0; i < filesCol.Length; i++)
                        {
                            ziparchive.CreateEntryFromFile(filesCol[i], _fileProvider.GetFileName(filesCol[i]));
                        }
                    }
                    return (memoryStream.ToArray(), "application/zip", fileName);
                }
            });
        }
    }
}