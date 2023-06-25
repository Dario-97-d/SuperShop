using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Helpers;
using SuperShop.Web.Models;

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


        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            // Get User

            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null) return;
            

            // Get Product

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null) return;
            

            // Get Order Detail of Product from Temporary Order Details

            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User.Id == user.Id && odt.Product.Id == product.Id)
                .FirstOrDefaultAsync();


            // Check Order Detail from Temporary Order Details exists

            if (orderDetailTemp == null)
            {
                // If not exists

                orderDetailTemp = new OrderDetailTemp
                {
                    User = user,
                    Product = product,
                    Price = product.Price,
                    Quantity = model.Quantity
                };

                await _context.OrderDetailsTemp.AddAsync(orderDetailTemp);
            }
            else
            {
                // If exists

                orderDetailTemp.Quantity += model.Quantity;
                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }

            await base.SaveAsync();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
                return;

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);
                await base.SaveAsync();
            }
        }

        public async Task<IQueryable<OrderDetailTemp>> GetOrderDetailsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
                return null;

            return _context.OrderDetailsTemp
                .AsNoTracking()
                .Where(odt => odt.User.Id == user.Id)
                .Include(odt => odt.Product)
                .OrderBy(odt => odt.Product.Name);
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
