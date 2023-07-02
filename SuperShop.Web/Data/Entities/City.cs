using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Data.Entities
{
    public class City : IEntity
    {
        // IEntity
        public int Id { get; set; }

        // City
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The City name must contain up to {1} characters.")]
        [Required]
        public string Name { get; set; }
    }
}
