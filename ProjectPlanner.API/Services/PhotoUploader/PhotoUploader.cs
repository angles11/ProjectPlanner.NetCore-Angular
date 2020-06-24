using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Services.PhotoUploader
{
    public class PhotoUploader : IPhotoUploader
    {
        private readonly PhotoUploaderSettings _photoUploaderSettings;

        public PhotoUploader(IOptions<PhotoUploaderSettings> photoUploaderSettings)
        {
            _photoUploaderSettings = photoUploaderSettings.Value;
        }
        public async Task<string> UploadPhotoAsync(IFormFile photo)
        {
            try
            {
                var connectionString = _photoUploaderSettings.ConnectionString;

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                var containerClient = blobServiceClient.GetBlobContainerClient("users");

                string fileName = Guid.NewGuid().ToString() + photo.FileName;

                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using Stream uploadFileStream = photo.OpenReadStream();

                await blobClient.UploadAsync(uploadFileStream, true);

               

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
