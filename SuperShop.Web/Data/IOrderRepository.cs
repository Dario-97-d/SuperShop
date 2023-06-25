using System.Linq;
using System.Threading.Tasks;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    public interface IOrderRepository
    {
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);
        Task<bool> ConfirmOrderAsync(string userName);
        Task DeleteDetailTempAsync(int id);
        Task<IQueryable<OrderDetailTemp>> GetOrderDetailsTempAsync(string userName);
        Task<IQueryable<Order>> GetOrdersAsync(string userName);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
    }
}
