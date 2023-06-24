using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace SuperShop.Web.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }


        [DisplayName("Full name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
