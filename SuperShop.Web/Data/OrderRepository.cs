using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;

namespace SuperShop.Web.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Order>> GetOrdersAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
                return null;

            if (await _userHelper.IsInRoleAsync(user, "Admin"))
            {
                return _context.Orders
                    .AsNoTracking()
                    .OrderByDescending(o => o.OrderDate)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product);
            }
            else
            {
                return _context.Orders
                    .AsNoTracking()
                    .Where(o => o.User.Id == user.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product);
            }
        }
    }
}
