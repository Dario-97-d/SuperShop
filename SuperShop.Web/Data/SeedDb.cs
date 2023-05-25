using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Web.Data
{
    public class SeedDb
    {
        readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Products.Any())
            {
                AddProduct("Smartphone");
                AddProduct("Smartwatch");
                AddProduct("Smartcar");
                AddProduct("Smartchair");

                await _context.SaveChangesAsync();
            }
        }

        void AddProduct(string productName)
        {
            var random = new Random();

            _context.Products.Add(new()
            {
                Name = productName,
                Price = random.Next(50) + 50,
                IsAvailable = true,
                Stock = random.Next(50)
            });
        }
    }
}
