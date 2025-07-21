using FoodioAPI.Configs;
using FoodioAPI.Database;
using FoodioAPI.Database.Repositories;
using FoodioAPI.Database.Repositories.Implements;
using FoodioAPI.Entities;
using FoodioAPI.Exceptions.Handler;
using FoodioAPI.Middlewares;
using FoodioAPI.Services;
using FoodioAPI.Services.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        //Google Config
        services.Configure<GoogleConfig>(configuration.GetSection("Google"));

        // Email Service
        services.Configure<EmailConfig>(configuration.GetSection("EmailApiKey"));
        services.AddScoped<IEmailService, EmailService>();

        // StorageService
        services.AddHttpClient();
        
        // Configure named HttpClient for API calls
        services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001/"); // Thay đổi port theo cấu hình của bạn
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        services.AddScoped<ApplicationDbContext>();
        //services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddSingleton<IStorageService>(s => new StorageService());
        services.AddScoped<IDineInMenuService, DineInMenuService>();
        services.AddScoped<IDineInOrderService, DineInOrderService>();
        services.AddScoped<ICashierOrderService, CashierOrderService>();

        // UnitOfWork, BaseRepository
        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Repository registrations
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IOrderSessionRepository, OrderSessionRepository>();

        // Auth related services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserManagementService, UserManagementService>();

        // Shift management services
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IDineInCartService, DineInCartService>();

        // Common services
        services.AddHttpClient("FoodioAPI", c =>
        {
            c.BaseAddress = new Uri("https://localhost:5001");
        });
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddRazorPages();

        // Add AutoMapper
        services.AddAutoMapper(typeof(Program));

        return services;
    }
}