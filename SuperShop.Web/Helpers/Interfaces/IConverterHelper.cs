using SuperShop.Web.Data.Entities;
using SuperShop.Web.ViewModels;

namespace SuperShop.Web.Helpers.Interfaces
{
    public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel pvm, bool isNew);

        ProductViewModel ToProductViewModel(Product product);
    }
}
