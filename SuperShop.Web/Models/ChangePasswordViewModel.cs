using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SuperShop.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DisplayName("Current password")]
        public string OldPassword { get; set; }


        [Required]
        [DisplayName("New password")]
        public string NewPassword { get; set; }


        [Required]
        [Compare("NewPassword")]
        [DisplayName("Confirm password")]
        public string Confirm { get; set; }
    }
}
