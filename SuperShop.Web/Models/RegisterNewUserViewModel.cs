using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuperShop.Web.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }


        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }


        [MaxLength(120, ErrorMessage = "The {0} must contain up to {1} characters.")]
        public string Address { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "A country is required.")]
        public int CountryId { get; set; }


        public IEnumerable<SelectListItem> Countries { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "A city is required.")]
        public int CityId { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set;}


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        [MinLength(8)]
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        [DisplayName("Confirm password")]
        public string Confirm { get; set; }
    }
}
