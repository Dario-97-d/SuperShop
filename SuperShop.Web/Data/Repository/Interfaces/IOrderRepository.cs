using System.Linq;
using System.Threading.Tasks;
using SuperShop.Web.Data.Entities;
using SuperShop.Web.ViewModels;

namespace SuperShop.Web.Data.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task AddItemToOrderAsync(AddItemViewModel model, User user);
        Task<bool> ConfirmOrderAsync(User user);
        Task DeleteDetailTempAsync(int id);
        Task DeliverOrder(DeliveryViewModel model);
        Task<Order> GetOrderAsync(int id);
        IQueryable<OrderDetailTemp> GetOrderDetailsTemp(User user);
        IQueryable<Order> GetOrders(User user);
        IQueryable<Order> GetOrdersAdmin();
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
    }
}
