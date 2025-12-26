using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;

namespace ShoeStoreWebApp.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ShoeStoreDbContext _context; 

        public IndexModel(ShoeStoreDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; } = new List<Product>(); 

        public List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();  

        [BindProperty(SupportsGet = true)]
        public string? SearchDescription { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedManufacturerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool HasDiscount { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool InStock { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        public async Task OnGetAsync()
        {
            // Загрузка всех производителей
            Manufacturers = await _context.Manufacturers.ToListAsync();

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .AsQueryable();

            // Фильтрация по описанию без учета регистра
            if (!string.IsNullOrEmpty(SearchDescription))
                query = query.Where(p => EF.Functions.Like(p.Description ?? "", $"%{SearchDescription}%"));

            // Фильтрация по производителю
            if (SelectedManufacturerId.HasValue)
                query = query.Where(p => p.ManufacturerId == SelectedManufacturerId.Value);

            // Фильтрация по максимальной цене
            if (MaxPrice.HasValue)
                query = query.Where(p => p.Price <= MaxPrice.Value);

            // Фильтрация товаров со скидкой
            if (HasDiscount)
                query = query.Where(p => p.Discount > 0);

            // Фильтрация товаров в наличии
            if (InStock)
                query = query.Where(p => p.Quantity > 0);

            // Сортировка
            query = SortBy switch
            {
                "name" => query.OrderBy(p => p.Category.Name),  // По названию
                "supplier" => query.OrderBy(p => p.Supplier.Name),  //По поставщику
                "price" => query.OrderBy(p => p.Price),  // По возрастанию цены
                "price-desc" => query.OrderByDescending(p => p.Price), // По убыванию цены
                _ => query
            };

            Products = await query.ToListAsync();
        }

        // Создание заказа
        public async Task<IActionResult> OnPostOrderAsync(int productId)
        {
            // Проверка аутентификации пользователя
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Login", new { returnUrl = "/Products/Index" });

            // Получение информации о пользователе
            var userLogin = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userLogin);
            if (user == null) 
                return NotFound("Пользователь не найден");

            // Создание заказа
            var order = new Order
            {
                UserId = user.UserId,
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(5),
                Code = new Random().Next(100, 999),
                IsFinished = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Добавление товара в заказ
            var orderProduct = new OrderProduct
            {
                OrderId = order.OrderId,
                ProductId = productId,
                Quantity = 1
            };

            _context.OrderProducts.Add(orderProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}