using FoodioAPI.Database;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodieAPII.Pages.Account
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public MenuItem SelectedItem { get; set; } // Món ăn hiện tại
        public List<MenuItem> SuggestedItems { get; set; } // Gợi ý món ăn cùng loại

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Truy vấn món ăn từ cơ sở dữ liệu theo ID
            SelectedItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);

            if (SelectedItem == null)
            {
                return NotFound();
            }

            // Truy vấn các món ăn có cùng CategoryId
            SuggestedItems = await _context.MenuItems
                .Where(m => m.CategoryId == SelectedItem.CategoryId && m.Id != id) // Lọc theo CategoryId và loại trừ món hiện tại
                .ToListAsync();

            return Page();
        }
    }
}
