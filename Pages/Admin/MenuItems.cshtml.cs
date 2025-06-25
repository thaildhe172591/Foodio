using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioClient.Pages.Admin;

//[Authorize(Roles = "Admin")]
public class MenuItemsModel : PageModel
{
    public void OnGet()
    {
    }
}