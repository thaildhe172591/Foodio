using Microsoft.AspNetCore.Identity;
using FoodioAPI.Database;
using FoodioAPI.Services.Implements;
using Microsoft.EntityFrameworkCore;
using FoodioAPI.Database.Repositories.Implements;
using FoodioAPI.Exceptions.Handler;
using FoodioAPI.DTOs.Configs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using FoodioAPI.Entities;
using FoodioAPI.Services;
using FoodioAPI.Database.Repositories;

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

        services.AddHttpClient();
        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddSingleton<IStorageService>(s => new StorageService());

        services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        services.AddRazorPages();
        services.AddExceptionHandler<CustomExceptionHandler>();
        //services.Configure<EmailConfig>(configuration.GetSection("EmailApiKey"));

        //services.AddAuthentication()
        //.AddGoogle(options =>
        //{
        //    options.ClientId = configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("ClientId is not configured");
        //    options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        //    options.CallbackPath = "/signin-google";
        //})
        //.AddFacebook(options =>
        //{
        //    options.AppId = configuration["Authentication:Facebook:AppId"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        //    options.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? throw new InvalidOperationException("ClientSecret is not configured");
        //    options.Events = new OAuthEvents
        //    {
        //        OnRemoteFailure = context =>
        //        {
        //            context.Response.Redirect("/Identity/Account/Login");
        //            context.HandleResponse();
        //            return Task.CompletedTask;
        //        }
        //    };
        //});
        //services.ConfigureApplicationCookie(options =>
        //{
        //    options.SlidingExpiration = true;
        //    options.ExpireTimeSpan = TimeSpan.FromDays(7);
        //});
        //services.Configure<CookiePolicyOptions>(options =>
        //{
        //    options.CheckConsentNeeded = context => false;
        //    options.MinimumSameSitePolicy = SameSiteMode.Lax;
        //});
        //services.ConfigureApplicationCookie(options =>
        //{
        //    options.LoginPath = "/Identity/Account/Login";
        //    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        //});


        //services.AddSingleton<NotificationQueueService>();
        //services.AddHostedService(sp => sp.GetService<NotificationQueueService>()!);

        return services;
    }
}