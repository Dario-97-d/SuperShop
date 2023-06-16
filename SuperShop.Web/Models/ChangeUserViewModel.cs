using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SuperShop.Web.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }


        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }
    }
}
