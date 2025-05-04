using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabProject.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Logout();
        }

        public IActionResult OnPost()
        {
            return Logout();
        }

        private IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            return RedirectToPage("Login");
        }
    }
}
