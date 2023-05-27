using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Web.Data
{
    public interface IGenericRespository<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(int id);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<bool> ExistsAsync(T entity);
    }
}
