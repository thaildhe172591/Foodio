using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodioClient.Pages.Admin;

[Authorize(Roles = "Admin")]
public class StatisticsModel : PageModel
{
    public void OnGet()
    {
    }
}