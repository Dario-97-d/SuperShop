﻿using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Data.Entities
{
    public class OrderDetailTemp : IEntity
    {
        // IEntity
        public int Id { get; set; }


        // OrderDetailTemp

        [Required]
        public User User { get; set; }


        [Required]
        public Product Product { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }


        public decimal Value => Price * (decimal)Quantity;
    }
}
