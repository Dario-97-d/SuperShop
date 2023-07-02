using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Data.Entities
{
    public class Country : IEntity
    {
        // IEntity
        public int Id { get; set; }

        // Country

        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "The Country name must contain up to {1} characters.")]
        [Required]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "Number of cities")]
        public int NumberCities => Cities?.Count ?? 0;
    }
}
