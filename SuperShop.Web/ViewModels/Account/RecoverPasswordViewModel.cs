using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.ViewModels.Account
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
