using FoodioAPI.Database;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;  // Thêm namespace cho Session

namespace FoodieAPII.Pages.Account
{
    public class HomePageLoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HomePageLoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MenuItem> HotDishes { get; set; }
        public List<MenuItem> ColdDishes { get; set; }
        public List<MenuItem> Drinks { get; set; }

        // Thông tin người dùng
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public void OnGet()
        {
            // Lấy thông tin người dùng từ Session
            UserName = HttpContext.Session.GetString("Username");
            UserEmail = HttpContext.Session.GetString("UserEmail");

            // Lấy danh sách món ăn theo CategoryId, không theo tên
            HotDishes = _context.MenuItems
                .Where(m => m.CategoryId == new Guid("b74d406f-7390-41ce-952c-12ceb4374064"))  // CategoryId cho "Món Nóng"
                .ToList();

            ColdDishes = _context.MenuItems
                .Where(m => m.CategoryId == new Guid("a4871081-e9da-4d5b-9596-aaea4981183a"))  // CategoryId cho "Món Lạnh"
                .ToList();

            Drinks = _context.MenuItems
                .Where(m => m.CategoryId == new Guid("3eaf6dd5-5a69-4e56-a478-2b6f32ae456c"))  // CategoryId cho "Nước Uống"
                .ToList();
        }
    }
}