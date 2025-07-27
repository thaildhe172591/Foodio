using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieAPII.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]
    public class SalesRevenueModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
