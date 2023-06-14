using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        
        [Required]
        [MinLength(8)]
        public string Password { get; set; }


        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }
}
