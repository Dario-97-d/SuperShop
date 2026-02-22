using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Data.Repository.Interfaces;
using SuperShop.Web.ViewModels;
using Vereyon.Web;

namespace SuperShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;

        public CountriesController(IFlashMessage flashMessage, ICountryRepository countryRepository)
        {
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
        }


        public async Task<IActionResult> AddCity(int? countryId)
        {
            if (countryId == null) return NotFound();

            var country = await _countryRepository.GetByIdAsync(countryId.Value);
            if (country == null) return NotFound();

            var model = new CityViewModel { CountryId = countryId.Value };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var city = new City { Name = model.Name };
                
                await _countryRepository.AddCityAsync(city, model.CountryId);

                return RedirectToAction(nameof(Details), new { id = model.CountryId});
            }

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.CreateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _flashMessage.Danger("This country is already registered.");
                }
            }

            return View(country);
        }

        public async Task<IActionResult> Delete (int? id)
        {
            if (id == null) return NotFound();

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null) return NotFound();

            try
            {
                await _countryRepository.DeleteAsync(country);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("DELETE") ?? false)
                {
                    ViewBag.ErrorTitle = $"Cannot delete this {nameof(Country)}";
                    ViewBag.ErrorMessage =
                        $"{country.Name} is included in some object relationship." +
                        $" Check and clear any relationships with {country.Name} and try again." +
                        $"<br />" +
                        $"<br />" +
                        $"Possible relationships include those with:" +
                        $"<br />" +
                        $"- {"Cities"}";
                }

                return View("Error");
            }
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null) return NotFound();

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null) return NotFound();

            try
            {
                var countryId = await _countryRepository.DeleteCityAsync(city.Id);
                return RedirectToAction("Details", new { id = countryId });
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("DELETE") ?? false)
                {
                    ViewBag.ErrorTitle = $"Cannot delete this {nameof(City)}";
                    ViewBag.ErrorMessage =
                        $"{city.Name} is included in some object relationship." +
                        $" Check and clear any relationships with {city.Name} and try again." +
                        $"<br />" +
                        $"<br />" +
                        $"Possible relationships include those with:" +
                        $"<br />" +
                        $"- {nameof(Data.Entities.User)}s";
                }

                return View("Error");
            }            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);
            if (country == null) return NotFound();

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null) return NotFound();

            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null) return NotFound();

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null) return NotFound();

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                var countryId = await _countryRepository.UpdateCityAsync(city);

                return RedirectToAction(nameof(Details), new { id = countryId });
            }

            return View(city);
        }

        public IActionResult Index()
        {
            return View(_countryRepository.GetCountriesWithCities());
        }
    }
}
