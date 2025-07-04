﻿using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ResumeAnalyzerMVC.Handlers;

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "", "ResumeAnalyzer");
var configPath = Path.Combine(solutionRoot, "appsettings.json");
builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        RoleClaimType = ClaimTypes.Role
    };
});


builder.Services.AddAuthorization();
builder.Services.AddSession();

builder.Services.AddHttpClient<AuthenticationHandler>();
builder.Services.AddHttpClient<ApplyHandler>();
builder.Services.AddHttpClient<ResumesHandler>();
builder.Services.AddHttpClient<ContactFormHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();
app.UseHttpsRedirection();


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();