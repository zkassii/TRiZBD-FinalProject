using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;

namespace ShoeStoreWebApp.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ShoeStoreLibrary.Contexts.ShoeStoreDbContext _context;

        public CreateModel(ShoeStoreLibrary.Contexts.ShoeStoreDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Login");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
