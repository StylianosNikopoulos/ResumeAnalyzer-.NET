using AuthService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Get connection string directly from configuration (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AuthenticationHandler>();

// Register session
builder.Services.AddSession();

// Register DbContext (using MySQL)
builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register API service
builder.Services.AddScoped<ApiService>();
builder.Services.AddHttpClient<ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseAuthentication();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
