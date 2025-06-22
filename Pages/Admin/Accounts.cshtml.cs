using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioClient.Pages.Admin;

//[Authorize(Roles = "Admin")]
public class AccountsModel : PageModel
{
    public void OnGet()
    {
    }
}