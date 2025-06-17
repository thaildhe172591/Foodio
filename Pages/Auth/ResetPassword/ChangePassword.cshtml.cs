using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoodioAPI.DTOs.Auth;
using FoodioAPI.Services;
using System.Threading.Tasks;

namespace FoodioAPI.Pages.Auth.ResetPassword
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IUserService _userService;

        public ChangePasswordModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }


        public void OnGet(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp!");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var changePasswordDTO = new ChangePasswordDTO
                {
                    Token = Token,
                    Email = Email,
                    Password = Password
                };

                var result = await _userService.ChangePasswordAsync(changePasswordDTO);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
                    return RedirectToPage("/auth/login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Đổi mật khẩu thất bại. Vui lòng thử lại!";
                }
            }

            return Page();
        }
    }
}
