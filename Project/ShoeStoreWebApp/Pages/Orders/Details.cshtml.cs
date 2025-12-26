using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;

namespace ShoeStoreWebApp.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly ShoeStoreLibrary.Contexts.ShoeStoreDbContext _context;

        public DetailsModel(ShoeStoreLibrary.Contexts.ShoeStoreDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);

            if (order is not null)
            {
                Order = order;

                return Page();
            }

            return NotFound();
        }
    }
}
