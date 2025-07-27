using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieAPII.Pages.Account
{
    [Authorize(Roles = "Customer")]
    public class TakeoutPaymentModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
