using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoeStoreWebApp.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var redirectUrl = Url.Page("/Products/Index") + "?ts=" + DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return Redirect(redirectUrl);
        }
        public IActionResult OnGet()
        {
            return RedirectToPage("/Products/Index");
        }
    }
}