using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Models;

namespace SuperShop.Web.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<User> GetUserByEmailAsync(string email);
        Task<SignInResult> LoginAsync(LoginViewModel viewModel);
        Task LogoutAsync();
        Task<IdentityResult> UpdateUserAsync(User user);
    }
}
