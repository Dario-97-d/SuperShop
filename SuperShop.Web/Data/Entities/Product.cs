using System;
using System.ComponentModel.DataAnnotations;
using SuperShop.Web.Models;

namespace SuperShop.Web.Data.Entities
{
    public class Product : IEntity
    {
        // IEntity

        public int Id { get; set; }

        // Product

        [Required]
        [MaxLength(31)]
        public string Name { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }


        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }


        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }


        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }


        public User User { get; set; }


        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty ?
            "https://myleasing.azurewebsites.net/images/no_image_icon.png" :
            "https://myleasingdariostorage.blob.core.windows.net/products/" + ImageId;


        public Product()
        {

        }

        public Product(ProductViewModel pvm)
        {
            Id = pvm.Id;
            ImageId = pvm.ImageId;
            IsAvailable = pvm.IsAvailable;
            LastPurchase = pvm.LastPurchase;
            LastSale = pvm.LastSale;
            Name = pvm.Name;
            Price = pvm.Price;
            Stock = pvm.Stock;
            User = pvm.User;
        }

    }
}
