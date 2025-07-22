using FoodioAPI;
using FoodioAPI.Database.Repositories.Implements;
using FoodioAPI.Exceptions.Handler;
using FoodioAPI.Mappings;
using FoodioAPI.Middlewares;
using FoodioAPI.Services.Implements;
using FoodioAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IDeliveryShipperService, DeliveryShipperService>();//Hieu add th�m

// Add Razor Pages + API Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");
app.UseMiddleware<TableTokenMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();    // API Routes (REST API)
app.MapRazorPages();     // Razor Pages (.cshtml Pages)

app.Run();
