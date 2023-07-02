﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Web.Data;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

namespace SuperShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICountryRepository _countryRepository;

        public AccountController(IUserHelper userHelper, ICountryRepository countryRepository)
        {
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
            if (user != null)
            {
                var country = await _countryRepository.GetCountryOfCityAsync(user.CityId);
                var countryId = country?.Id ?? 0;

                var model = new ChangeUserViewModel
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
        [HttpPost, ActionName("ChangeUser")]
        public async Task<IActionResult> ChangeUserAsync(ChangeUserViewModel model)
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
                    return RedirectToAction(nameof(ChangeUserAsync));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }

            return View(model);
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
