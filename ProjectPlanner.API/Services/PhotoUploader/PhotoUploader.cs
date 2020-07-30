using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Services.PhotoUploader
{
    public class PhotoUploader : IPhotoUploader
    {
        private readonly PhotoUploaderSettings _photoUploaderSettings;

        BlobContainerClient containerClient;
        public PhotoUploader(IOptions<PhotoUploaderSettings> photoUploaderSettings)
        {
            _photoUploaderSettings = photoUploaderSettings.Value;

            var connectionString = _photoUploaderSettings.ConnectionString;

            var blobServiceClient = new BlobServiceClient(connectionString);

            containerClient = blobServiceClient.GetBlobContainerClient("users");
        }
        public async Task<string> UploadPhotoAsync(IFormFile photo)
        {
            try
            {
                
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

        public async Task<bool> DeletePhotoAsync (string photoUrl)
        {
            string blobName = new CloudBlockBlob(new Uri(photoUrl)).Name;

            BlobClient blobClient = containerClient.GetBlobClient(blobName);


            return await blobClient.DeleteIfExistsAsync();

        }
    }
}
