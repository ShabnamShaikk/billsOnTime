using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project1.Pages
{
    public class LogOffModel : PageModel
    {
        public void OnGet()
        {
            Response.Cookies.Delete("Username");
            Response.Redirect("signout");
        }
    }
}
