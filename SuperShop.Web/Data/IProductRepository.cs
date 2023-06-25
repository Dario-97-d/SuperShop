using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public interface IProductRepository : IGenericRespository<Product>
    {
        IQueryable GetAllWithUsers();
        Task<IEnumerable<SelectListItem>> GetComboProductsAsync();
    }
}
