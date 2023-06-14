using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SuperShop.Web.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            // Setting Blob connection configuration

            string keys = configuration["Blob:ConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Guid> UploadBlobAsync(IFormFile image, string containerName)
        {
            Stream stream = image.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] image, string containerName)
        {
            var stream = new MemoryStream(image);
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }

        async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            var guid = Guid.NewGuid();

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob block = container.GetBlockBlobReference($"{guid}");

            await block.UploadFromStreamAsync(stream);

            return guid;
        }
    }
}
