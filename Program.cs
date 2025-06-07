using FoodioAPI;
using FoodioAPI.Database.Repositories.Implements;
using FoodioAPI.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
// Add Razor Pages + API Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();    // API Routes (REST API)
app.MapRazorPages();     // Razor Pages (.cshtml Pages)

app.Run();
