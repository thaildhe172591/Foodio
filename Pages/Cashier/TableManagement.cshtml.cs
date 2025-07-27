using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieAPII.Pages.Cashier
{
    [Authorize(Roles = "Cashier")]
    public class TableManagementModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
