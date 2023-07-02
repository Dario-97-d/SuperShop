using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;

namespace SuperShop.Web.Data
{
    public class SeedDb
    {
        readonly DataContext _context;
        readonly IUserHelper _userHelper;


        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }


        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            await SeedCountriesAndCities();

            var user = await SeedUser();

            if (!_context.Products.Any())
            {
                AddProduct("Smartphone", user);
                AddProduct("Smartwatch", user);
                AddProduct("Smartcar", user);
                AddProduct("Smartchair", user);

                await _context.SaveChangesAsync();
            }
        }


        void AddProduct(string productName, User user)
        {
            var random = new Random();

            _context.Products.Add(new()
            {
                Name = productName,
                Price = random.Next(50) + 50,
                IsAvailable = true,
                Stock = random.Next(50),
                User = user,
                ImageId = Guid.Empty
            });
        }

        async Task SeedCountriesAndCities()
        {
            if (!await _context.Countries.AnyAsync())
            {
                var country = (await _context.Countries.AddAsync(new Country { Name = "Portugal" })).Entity;
                country.Cities = new List<City>
                {
                    new City { Name = "Porto" },
                    new City { Name = "Lisboa" }
                };

                await _context.SaveChangesAsync();
            }
        }

        async Task<User> SeedUser()
        {
            string defaultEmail = "dario@e.mail";
            string password = defaultEmail;

            User user = await _userHelper.GetUserByEmailAsync(defaultEmail);

            // If user doesn't exist, create it
            if (user == null)
            {
                var city = _context.Countries.FirstOrDefault().Cities.FirstOrDefault();

                user = new()
                {
                    FirstName = "Dário",
                    LastName = "d97",
                    UserName = defaultEmail,
                    Email = defaultEmail,
                    PhoneNumber = "987654321",
                    Address = "Casa na rua",
                    City = city,
                    CityId = city.Id
                };

                var result = await _userHelper.AddUserAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Could not create user in seeder." +
                        "\n" + result.Errors
                        );
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            return user;
        }
    }
}
