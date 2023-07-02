using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SuperShop.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(60, ErrorMessage = "The {0} must contain up to {1} characters.")]
        public string FirstName { get; set; }


        [MaxLength(60, ErrorMessage = "The {0} must contain up to {1} characters.")]
        public string LastName { get; set; }


        [MaxLength(120, ErrorMessage = "The {0} must contain up to {1} characters.")]
        public string Address { get; set; }


        public int CityId { get; set; }

        public City City { get; set; }


        // Readonly

        [DisplayName("Full name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
