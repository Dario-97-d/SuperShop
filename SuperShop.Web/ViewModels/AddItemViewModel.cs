using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperShop.Web.ViewModels
{
    public class AddItemViewModel
    {
        [Display(Name = "Product")]
        [Range(1, int.MaxValue, ErrorMessage = "You need to select a product.")]
        public int ProductId { get; set; }


        [Range(0.001, double.MaxValue, ErrorMessage = "The quantity is not valid.")]
        public int Quantity { get; set; }


        public IEnumerable<SelectListItem> Products { get; set; }
    }
}
