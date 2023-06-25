using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Web.Data.Entities;

namespace SuperShop.Web.Data
{
    public interface IProductRepository : IGenericRespository<Product>
    {
        IQueryable GetAllWithUsers();
        IEnumerable<SelectListItem> GetComboProducts();
    }
}
