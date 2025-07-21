using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioAPI.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserManagerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
