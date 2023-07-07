using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SuperShop.Web.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Country : IEntity
    {
        // IEntity
        public int Id { get; set; }

        // Country

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "The Country name must contain up to {1} characters.")]
        [Required]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "Number of cities")]
        public int NumberCities => Cities?.Count ?? 0;
    }
}
