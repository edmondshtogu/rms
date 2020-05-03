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
                if (postedFileBase == null) return string.Empty;

                var fileName = _fileProvider.GetFileNameWithoutExtension(postedFileBase.FileName);
                var fileExtension = _fileProvider.GetFileExtension(postedFileBase.FileName);
                fileName = $"{DateTime.Now:yyyyMMddhhmmss}-{fileName.Trim()}{fileExtension}";

                var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);
                storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

                _fileProvider.CreateDirectory(storagePath);

                storagePath = _fileProvider.Combine(storagePath, fileName);

                postedFileBase.SaveAs(storagePath);

                return _fileProvider.GetFileName(postedFileBase.FileName);
            });
        }

        public async Task<byte[]> GetAttachmentsOwnedByAsync(Guid ownerId)
        {
            return await Task.Run(() =>
            {

                var storagePath = _fileProvider.MapPath(_appConfig.StoragePath);

                storagePath = _fileProvider.Combine(storagePath, ownerId.ToString());

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
                    return memoryStream.ToArray();
                }
            });
        }
    }
}