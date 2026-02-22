using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperShop.Web.Helpers.Interfaces
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(byte[] image, string containerName);
        Task<Guid> UploadBlobAsync(IFormFile image, string containerName);
        Task<Guid> UploadBlobAsync(string image, string containerName);
    }
}