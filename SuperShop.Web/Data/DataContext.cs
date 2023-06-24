using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        // Orders

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderDetailTemp> OrderDetailsTemp { get; set; }

        // Products

        public DbSet<Product> Products { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
