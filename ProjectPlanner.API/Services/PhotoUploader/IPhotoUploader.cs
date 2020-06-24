using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Services.PhotoUploader
{
    public interface IPhotoUploader
    {
        Task<string> UploadPhotoAsync(IFormFile photo);
    }
}
