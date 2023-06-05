using SuperShop.Web.Data.Entities;
using SuperShop.Web.Models;

namespace SuperShop.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Product ToProduct(ProductViewModel pvm, bool isNew)
        {
            return new()
            {
                Id = isNew ? 0 : pvm.Id,
                ImageUrl = pvm.ImageUrl,
                IsAvailable = pvm.IsAvailable,
                LastPurchase = pvm.LastPurchase,
                LastSale = pvm.LastSale,
                Name = pvm.Name,
                Price = pvm.Price,
                Stock = pvm.Stock,
                User = pvm.User
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new()
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }
    }
}
