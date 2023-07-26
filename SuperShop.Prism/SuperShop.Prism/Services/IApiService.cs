using System.Threading.Tasks;
using SuperShop.Prism.Models;

namespace SuperShop.Prism.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller);
    }
}