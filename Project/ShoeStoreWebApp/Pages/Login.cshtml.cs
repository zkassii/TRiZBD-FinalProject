using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System.Security.Claims;

namespace ShoeStoreWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ShoeStoreDbContext _context;

        public LoginModel(ShoeStoreDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Login { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Page("/Index");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Page("/Index");

            // Поиск пользователя в бд
            var user = await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Login == Login && u.PasswordHash == Password);

            if (user != null)
            {
                // Список для аутентификации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, user.Role.Name), 
                    new Claim("FullName", $"{user.FirstName} {user.SecondName} {user.MiddleName}".Trim())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return Page();
        }
    }
}