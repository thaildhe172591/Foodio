using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieAPII.Pages.Account
{
    [Authorize(Roles = "Customer")]
    public class CartModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
