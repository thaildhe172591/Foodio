using FoodioAPI.DTOs.Auth;
using FoodioAPI.DTOs;
using FoodioAPI.DTOs.Token;
using FoodioAPI.DTOs.User;
using Microsoft.AspNetCore.Identity;
namespace FoodioAPI.Services;

public interface IUserService
{
    Task<UserDTO?> FindOrCreateUserAsync(ExternalAuthDTO externalAuth, List<string> roles);
    Task<TokenDTO> CreateAuthTokenAsync(string userName, int expDays = -1);
    Task<TokenDTO> RefeshAuthTokenAsync(string refeshToken);
    Task<string> GeneratePasswordResetTokenAsync(string email);

    Task<bool> CheckEmailExistedAsync(string email);
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO model);
    Task RemoveRefreshTokenAsync(string refreshToken);
    Task<string?> GetRefreshTokenAsync(string userName);
    Task<UserDTO> LockUser(LockUserDTO lockUserDTO);
    Task<UserDTO> UnlockUser(LockUserDTO lockUserDTO);
    Task<bool> IsUserLocked(string userName);
    Task<DateTimeOffset?> GetUnlockTime(string userName);
}
