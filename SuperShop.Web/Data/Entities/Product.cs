﻿using System;
using System.ComponentModel.DataAnnotations;

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


        [Display(Name = "Image")]
        public string ImageUrl { get; set; }


        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }

        
        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }


        public User User { get; set; }

        public string ImageFullPath =>
            string.IsNullOrEmpty(ImageUrl) ? null : "https://localhost:44389/" + ImageUrl[1..];

    }
}
