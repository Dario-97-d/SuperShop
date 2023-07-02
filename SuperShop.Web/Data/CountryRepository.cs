using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public async Task AddCityAsync(City city, int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId);
            if (country != null)
            {
                country.Cities ??= new List<City>();
                country.Cities.Add(city);
                _context.Countries.Update(country);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteCityAsync(int id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Cities.Any(ci => ci.Id == id));

            if (country != null)
            {
                var city = await _context.Cities.FindAsync(id);
                if (city != null)
                {
                    _context.Cities.Remove(city);

                    await _context.SaveChangesAsync();

                    return country.Id;
                }
            }

            return 0;
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCitiesOfCountryAsync(int countryId)
        {
            var country = await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == countryId);

            var list = new List<SelectListItem>();

            if (country != null && country.Cities != null)
            {
                list = country.Cities.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a city)",
                    Value = "0"
                });
            }
            else
            {
                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a country)",
                    Value = "0"
                });
            }

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCountriesAsync()
        {
            var list = await _context.Countries.AsNoTracking().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a country)",
                Value = "0"
            });

            return list;
        }

        public IQueryable<Country> GetCountriesWithCities()
        {
            return _context.Countries.AsNoTracking().Include(c => c.Cities);
        }

        public async Task<Country> GetCountryOfCityAsync(int cityId)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(c => c.Cities.Any(ci => ci.Id == cityId));
        }

        public async Task<Country> GetCountryWithCitiesAsync(int id)
        {
            return await _context.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Cities.Any(ci => ci.Id == city.Id));

            if (country != null)
            {
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();

                return country.Id;
            }

            return 0;
        }
    }
}
