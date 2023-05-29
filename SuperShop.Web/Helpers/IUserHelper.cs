using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<User> GetUserByEmailAsync(string email);
    }
}
