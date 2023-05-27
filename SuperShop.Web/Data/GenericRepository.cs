using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data.Entities;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SuperShop.Web.Data
{
    public class GenericRepository<T>: IGenericRespository<T> where T : class, IEntity
    {
        DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }


        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }

        public async Task<bool> ExistsAsync(T entity)
        {
            return await _context.Set<T>().AnyAsync(e => e == entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
