using System.Linq;
using System.Threading.Tasks;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public interface IOrderRepository
    {
        Task<IQueryable<Order>> GetOrdersAsync(string userName);
    }
}
