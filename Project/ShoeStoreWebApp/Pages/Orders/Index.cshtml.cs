using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System.Security.Claims;

namespace ShoeStoreWebApp.Pages.Orders
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ShoeStoreDbContext _context;

        public IndexModel(ShoeStoreDbContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            // Получение логина
            var userLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            // Проверка наличия логина
            if (string.IsNullOrEmpty(userLogin))
            {
                Orders = new List<Order>();
                return;
            }

            var currentUser = await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Login == userLogin);

            // Проверка существования пользователя в базе
            if (currentUser == null)
            {
                Orders = new List<Order>();
                return;
            }

            // Запрос для получения заказов
            IQueryable<Order> ordersQuery = _context.Orders
                .Include(o => o.User) 
                .Include(o => o.OrderProducts!)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.OrderProducts!)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Manufacturer)
                .OrderByDescending(o => o.OrderDate);

            // Проверка роли пользователя
            if (User.IsInRole("Администратор") || User.IsInRole("Менеджер"))
            {
                Orders = await ordersQuery.ToListAsync();
            }
            else
            {
                Orders = await ordersQuery
                    .Where(o => o.UserId == currentUser.UserId)
                    .ToListAsync();
            }
        }
    }
}