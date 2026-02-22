using System.Threading.Tasks;
using SuperShop.Web.Data.Repository;

namespace SuperShop.Web.Data
{
    public interface IUnitOfWork
    {
        CountryRepository CountryRepository { get; }
        OrderRepository OrderRepository { get; }
        ProductRepository ProductRepository { get; }

        Task SaveAsync();
    }
}