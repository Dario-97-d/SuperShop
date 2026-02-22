using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperShop.Web.Helpers.Interfaces
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile image, string folder);
    }
}
