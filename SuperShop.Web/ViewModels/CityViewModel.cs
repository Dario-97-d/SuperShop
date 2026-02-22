using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.ViewModels
{
    public class CityViewModel
    {
        public int CountryId { get; set; }

        public int CityId { get; set; }

        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The City name must contain up to {1} characters.")]
        [Required]
        public string Name { get; set; }
    }
}
