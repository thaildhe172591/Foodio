using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoodioAPI.Configs;
using FoodioAPI.DTOs.Auth;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FoodioAPI.Services;

namespace FoodioAPI.Services.Implements;

public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly GoogleConfig _googleConfig;
    private readonly SymmetricSecurityKey _key;
    public TokenService(IOptions<JwtConfig> jwtConfig, IOptions<GoogleConfig> googleConfig)
    {
        _jwtConfig = jwtConfig.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SigningKey));
        _googleConfig = googleConfig.Value;
    }

    public string GenerateAccessToken(List<Claim> claims)
    {
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new(claims),
            Expires = DateTime.Now.AddMinutes(_jwtConfig.TokenValidityInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };
        var tokenHandle = new JwtSecurityTokenHandler();
        var token = tokenHandle.CreateToken(tokenDescriptor);
        return tokenHandle.WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SigningKey)),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(ExternalAuthDTO externalAuth)
    {
        try
        {
            Console.WriteLine("Validating Google token...");
            Console.WriteLine($"_googleConfig.ClientId = '{_googleConfig.ClientId}'");
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [_googleConfig.ClientId],
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
            Console.WriteLine("Google token is valid.");
            return payload;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Google token validation failed:");
            Console.WriteLine(ex.ToString()); 
            return null;
        }
    }   
}
