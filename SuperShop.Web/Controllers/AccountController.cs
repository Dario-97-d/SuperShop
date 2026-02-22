using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Data.Repository.Interfaces;
using SuperShop.Web.Helpers.Interfaces;
using SuperShop.Web.ViewModels.Account;
using SuperShop.Web.Utils;

namespace SuperShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICountryRepository _countryRepository;

        public AccountController(
            IConfiguration configuration,
            IMailHelper mailHelper,
            IUserHelper userHelper,
            ICountryRepository countryRepository)
        {
            _configuration = configuration;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
            _countryRepository = countryRepository;
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


        public async Task<IActionResult> Register()
        {
            var model = new RegisterNewUserViewModel
            {
                Countries = await _countryRepository.GetComboCountriesAsync(),
                Cities = await _countryRepository.GetComboCitiesOfCountryAsync(0)
            };
            return View(model);
        }


        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterNewUserViewModel model)
        {
            // Reset model's Countries' and Cities' lists
            // for View(model)
            // because POST doesn't bring them through
            model.Countries = await _countryRepository.GetComboCountriesAsync();
            model.Cities = await _countryRepository.GetComboCitiesOfCountryAsync(model.CountryId);

            if (ModelState.IsValid)
            {
                // Checking user is already registered

                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "This user is already registered.");
                    return View(model);
                }

                // Check City

                var city = await _countryRepository.GetCityAsync(model.CityId);
                if (city == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid city.");
                    return View(model);
                }

                // Converting RegisterViewModel to User

                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Username,
                    UserName = model.Username,
                    Address = model.Address,
                    CityId = model.CityId,
                    City = city,
                };

                // Creating user

                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Could not create User.");
                    return View(model);
                }

                await _userHelper.AddUserToRoleAsync(user, "Customer");

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token
                }, protocol: HttpContext.Request.Scheme);

                Response sendEmail = _mailHelper.SendEmail(
                    user.Email,
                    "Email confirmation",
                    
                    "<h2>Email confirmation</h2>" +
                    "<p>Click this link to activate account:</p>" +
                    "<a href=\""+tokenLink+"\">Confirm email</a>");

                if (sendEmail.IsSuccess)
                {
                    ViewBag.Message =
                        "Account confirmation instructions " +
                        "have been sent to the registered email address.";
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Could not send email for account confirmation.");
                }

                return View(model);
            }

            // !ModelState.IsValid
            ModelState.AddModelError(string.Empty, "Could not register account.");
            return View(model);
        }


        [Authorize]
        [ActionName("EditUser")]
        public async Task<IActionResult> EditUserAsync()
        {
            ViewBag.UserMessage = TempData["UserMessage"] ?? "";

            // Getting user

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user != null)
            {
                var country = await _countryRepository.GetCountryOfCityAsync(user.CityId);
                var countryId = country?.Id ?? 0;

                var model = new EditUserViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    CountryId = countryId,
                    Countries = await _countryRepository.GetComboCountriesAsync(),
                    CityId = user.CityId,
                    Cities = await _countryRepository.GetComboCitiesOfCountryAsync(countryId),
                };

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        [HttpPost, ActionName("EditUser")]
        public async Task<IActionResult> EditUserAsync(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Getting user

                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var city = await _countryRepository.GetCityAsync(user.CityId);
                if (city == null)
                {
                    ModelState.AddModelError(string.Empty, "Could not find City.");
                }
                else
                {
                    // Getting new user values

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.CityId = model.CityId;
                    user.City = city;

                    // Updating user

                    var updateUser = await _userHelper.UpdateUserAsync(user);
                    if (updateUser.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, updateUser.Errors.FirstOrDefault().Description);
                    }

                    // Reset model's Countries' and Cities' lists
                    // because POST doesn't bring them through
                    model.Countries = await _countryRepository.GetComboCountriesAsync();
                    model.Cities = await _countryRepository.GetComboCitiesOfCountryAsync(model.CountryId);
                }
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
                    return RedirectToAction(nameof(EditUserAsync));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }

            return View(model);
        }


        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is not registered.");
                    return View(model);
                }

                var token = await _userHelper.GeneratePasswordResetTolenAsync(user);

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token },
                    protocol: HttpContext.Request.Scheme);

                var emailBody = "<h2>Reset password</h2>" +
                    "<p>Click the link to reset password:</p>" +
                    "<a href=\"" + link + "\">Reset password</a>";

                Response sendEmail = _mailHelper.SendEmail(
                    model.Email,
                    "Password Recovery",
                    emailBody);

                if (sendEmail.IsSuccess)
                {
                    ViewBag.Message =
                        "Password recovery instructions " +
                        "have been sent to the registered email.";
                }
                else ModelState.AddModelError(string.Empty, "Could not send password recovery email.");

                return View();
            }

            ModelState.AddModelError(string.Empty, "Something went wrong.");
            return View(model);
        }


        public IActionResult ResetPassword(string token)
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    var resetPassword = await _userHelper.ResetPassword(user, model.Token, model.Password);
                    if (resetPassword.Succeeded)
                    {
                        ViewBag.Message = "Password reset successful!";
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Could not reset password.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Could not find user.");
                    return View(model);
                }
            }
            
            ModelState.AddModelError(string.Empty, "Could not reset password.");
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var validatePassword = await _userHelper
                        .ValidatePasswordAsync(user, model.Password);

                    if (validatePassword.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }


        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token))
                return NotFound();

            // Get User by Id

            var user = await _userHelper.GetUserByIdAsync(userid);
            if (user == null) return NotFound();

            // Confirm Email

            var confirmEmail = await _userHelper.ConfirmEmailAsync(user, token);
            if (!confirmEmail.Succeeded) return NotFound();

            // Success
            return View();
        }


        [HttpPost]
        [Route("Account/GetCitiesOfCountryAsync")]
        public async Task<JsonResult> GetCitiesOfCountryAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);
            return Json(country.Cities.OrderBy(c => c.Name));
        }

        public IActionResult NotAuthorized()
        {
            // Startup.ConfigureServices():
            //  services.ConfigureAplicationCookie(options.+Path = NotAuthorized())
            return View();
        }
    }
}
