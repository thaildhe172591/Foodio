using System.Security.Claims;
using AutoMapper;
using FoodioAPI.DTOs.Token;
using FoodioAPI.DTOs.User;
using FoodioAPI.Entities;
using FoodioAPI.Database.Repositories;
using Microsoft.AspNetCore.Identity;
using FoodioAPI.DTOs.Auth;
using FoodioAPI.Exceptions;
using FoodioAPI.Helpers;
using FoodioAPI.DTOs;
using FoodioAPI.Services;

namespace FoodioAPI.Services.Implements;
public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<User> userManager,
        ITokenService tokenService,
        IUserRepository userRepository,
        IMapper mapper
    )
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _mapper = mapper;
    }


    private async Task<TokenDTO> CreateAuthTokenAsync(User user, int expDays = -1)
    {
        user.RefreshToken = _tokenService.GenerateRefreshToken();
        if (expDays > 0)
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expDays);
        await _userManager.UpdateAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName??string.Empty),
            new(ClaimTypes.Email, user.Email??string.Empty)
        };
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        return new TokenDTO()
        {
            AccessToken = _tokenService.GenerateAccessToken(claims),
            RefreshToken = user.RefreshToken,
            Role = roles.ToList()
        };
    }

    public async Task<TokenDTO> CreateAuthTokenAsync(string userName, int expDays = -1)
    {
        var user = await _userManager.FindByNameAsync(userName)
            ?? throw new Exception("UserName is invalid");
        return await CreateAuthTokenAsync(user, expDays);
    }
    public async Task<TokenDTO> RefeshAuthTokenAsync(string refeshToken)
    {
        var user = await _userRepository.FindUserByRefreshTokenAsync(refeshToken);
        if (user is null || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new Exception("Invalid access token or refresh token");
        return await CreateAuthTokenAsync(user);
    }

    public async Task<UserDTO?> FindOrCreateUserAsync(ExternalAuthDTO externalAuth, List<string> roles)
    {
        var payload = await _tokenService.VerifyGoogleToken(externalAuth);
        //if (payload == null || !EmailHelper.IsFptMail(payload.Email)) throw new Exception("Email must be FPT email");
        if (payload == null) throw new Exception("Invalid Google Token");
        var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(payload.Email);
            //if (user == null)
            //{
            //    user = new User { Email = payload.Email, UserName = EmailHelper.GetUsername(payload.Email), EmailConfirmed = true };
            //    await _userManager.CreateAsync(user);
            //    await _userManager.AddToRolesAsync(user, roles);
            //    await _userManager.AddLoginAsync(user, info);
            //}
            //else
            //    throw new Exception("Email already exists");
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    UserName = EmailHelper.GetUsername(payload.Email),
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(user);
                await _userManager.AddToRolesAsync(user, roles);
                await _userManager.AddLoginAsync(user, info);
            }
            else
            {
                var logins = await _userManager.GetLoginsAsync(user);
                var hasGoogleLogin = logins.Any(x => x.LoginProvider == externalAuth.Provider);
                if (!hasGoogleLogin)
                    await _userManager.AddLoginAsync(user, info);
            }

        }
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> CheckEmailExistedAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user != null;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email) ?? throw new Exception("User not found");
        return await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
    }

    public async Task<IdentityResult> DirectUpdatePasswordAsync(DirectUpdatePasswordDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email) ?? throw new Exception("User not found");
        
        // Tạo token để reset password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Sử dụng ResetPasswordAsync để update password mới
        return await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
    }
    public async Task RemoveRefreshTokenAsync(string refreshToken)

    {
        var appUser = await _userRepository.FindUserByRefreshTokenAsync(refreshToken);
        if (appUser == null)
        {
            Console.WriteLine("User not found");
            return;
        }
        appUser.RefreshToken = "";
        await _userManager.UpdateAsync(appUser);
    }

    public async Task<string?> GetRefreshTokenAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user?.RefreshToken;
    }

    public async Task<UserDTO> LockUser(LockUserDTO lockUserDTO)
    {
        if (string.IsNullOrEmpty(lockUserDTO.UserName) || lockUserDTO.LockoutDays <= 0)
            throw new Exception("Invalid input");
        var user = await _userManager.FindByNameAsync(lockUserDTO.UserName)
            ?? throw new Exception("User not found");
        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(lockUserDTO.LockoutDays));
        if (result.Succeeded) throw new Exception("Lockout failed");
        return _mapper.Map<UserDTO>(user);

    }

    public async Task<UserDTO> UnlockUser(LockUserDTO lockUserDTO)
    {
        if (string.IsNullOrEmpty(lockUserDTO.UserName))
        {
            throw new Exception("Invalid input");
        }
        var user = await _userManager.FindByNameAsync(lockUserDTO.UserName)
            ?? throw new Exception("User not found");
        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        if (result.Succeeded)
        {
            return _mapper.Map<UserDTO>(user);
        }
        else
        {
            throw new Exception("Lockout failed");
        }
    }
    public async Task<bool> IsUserLocked(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName)
            ?? throw new ApplicationException($"User '{userName}' not found.");
        return await _userManager.IsLockedOutAsync(user);
    }
    public async Task<DateTimeOffset?> GetUnlockTime(string userName)   
    {
        var user = await _userManager.FindByNameAsync(userName)
            ?? throw new ApplicationException($"User '{userName}' not found.");
        return await _userManager.GetLockoutEndDateAsync(user);
    }
}
