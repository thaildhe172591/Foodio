using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieAPII.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]
    public class OrderOnlineCashierModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
