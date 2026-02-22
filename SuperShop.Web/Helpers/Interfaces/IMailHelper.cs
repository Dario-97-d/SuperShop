using SuperShop.Web.Utils;

namespace SuperShop.Web.Helpers.Interfaces
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body);
    }
}