using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
