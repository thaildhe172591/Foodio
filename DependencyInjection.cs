using Microsoft.AspNetCore.Identity;
using FoodioAPI.Database;
using FoodioAPI.Services.Implements;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.Database.Repositories.Implements;
using FoodioAPI.Exceptions.Handler;
using FoodioAPI.Configs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using FoodioAPI.Entities;
using FoodioAPI.Services;
using FoodioAPI.Database.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FoodioAPI;
public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase =
                options.Password.RequireLowercase =
                    options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider;
        }
        ).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT config
        var jwtSection = configuration.GetSection("JWT");
        services.Configure<JwtConfig>(jwtSection);
        var jwtConfig = jwtSection.Get<JwtConfig>() ?? throw new Exception("Jwt options have not been set!");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.SigningKey))
            };
            option.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Request.Cookies.TryGetValue(jwtConfig.AccessTokenKey, out var accessToken);
                    if (!string.IsNullOrEmpty(accessToken))
                        context.Token = accessToken;
                    return Task.CompletedTask;
                }
            };
        });

        // Email Service
        services.Configure<EmailConfig>(configuration.GetSection("EmailApiKey"));
        services.AddScoped<IEmailService, EmailService>();

        // StorageService
        services.AddSingleton<IStorageService>(s => new StorageService());

        // UnitOfWork, BaseRepository
        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Auth related services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserRepository, UserRepository>();


        // Common services
        services.AddHttpClient();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddRazorPages();

        // Add AutoMapper
        services.AddAutoMapper(typeof(Program));

        return services;
    }
}