using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signIn = await _userHelper.LoginAsync(model);
                if (signIn.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }


        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Checking user is already registered

                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "This user is already registered.");
                    return View(model);
                }

                // Converting RegisterViewModel to User

                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Username,
                    UserName = model.Username
                };

                await _userHelper.AddUserToRoleAsync(user, "Customer");

                // Creating user

                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Could not create User.");
                    return View(model);
                }

                // Preparing automatic login

                var login = new LoginViewModel
                {
                    Username = model.Username,
                    Password = model.Password,
                    RememberMe = false
                };

                return await LoginAsync(login);

                //var resultLogin = await _userHelper.LoginAsync(login);
                //if (!resultLogin.Succeeded)
                //{
                //    ModelState.AddModelError(string.Empty, "Could not login.");
                //    return View(model);
                //}

                //// Success
                //return RedirectToAction("Index", "Home");
            }

            // !ModelState.IsValid

            return View(model);
        }


        [Authorize]
        [ActionName("ChangeUser")]
        public async Task<IActionResult> ChangeUserAsync()
        {
            ViewBag.UserMessage = TempData["UserMessage"] ?? "";

            // Getting user

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ChangeUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(model);
        }


        [Authorize]
        [HttpPost, ActionName("ChangeUser")]
        public async Task<IActionResult> ChangeUserAsync(ChangeUserViewModel model)
        {
            // Getting user

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Getting new user values

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                // Updating user

                var result = await _userHelper.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    ViewBag.UserMessage = "User updated!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }

                return View(model);
            }

            return View(model);
        }


        public IActionResult ChangePassword()
        {
            return View();
        }


        [Authorize]
        [HttpPost, ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            // Getting user

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["UserMessage"] = "Password changed.";
                    return RedirectToAction(nameof(ChangeUserAsync));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }

            return View(model);
        }


        public IActionResult NotAuthorized()
        {
            // Startup.ConfigureServices():
            //  services.ConfigureAplicationCookie(options.+Path = NotAuthorized())
            return View();
        }
    }
}
