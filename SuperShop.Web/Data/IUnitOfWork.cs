using System.Threading.Tasks;

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