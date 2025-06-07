using System.Security.Claims;
using FoodioAPI.DTOs.Auth;
using Google.Apis.Auth;

namespace FoodioAPI.Services;

public interface ITokenService
{
    string GenerateRefreshToken();
    string GenerateAccessToken(List<Claim> claims);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(ExternalAuthDTO externalAuth);
}
