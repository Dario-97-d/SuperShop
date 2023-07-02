using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public interface ICountryRepository : IGenericRespository<Country>
    {
        Task AddCityAsync(City city, int countryId);
        Task<int> DeleteCityAsync(int id);
        Task<City> GetCityAsync(int id);
        Task<IEnumerable<SelectListItem>> GetComboCitiesOfCountryAsync(int countryId);
        Task<IEnumerable<SelectListItem>> GetComboCountriesAsync();
        IQueryable<Country> GetCountriesWithCities();
        Task<Country> GetCountryOfCityAsync(int cityId);
        Task<Country> GetCountryWithCitiesAsync(int id);
        Task<int> UpdateCityAsync(City city);
    }
}