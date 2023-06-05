using System.Linq;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        readonly DataContext _context;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Products.Include(p => p.User);
        }
    }
}
