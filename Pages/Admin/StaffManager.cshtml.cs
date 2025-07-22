using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioAPI.Pages.Admin
{
    //[Authorize(Roles = "Admin")]
    public class StaffManagerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
