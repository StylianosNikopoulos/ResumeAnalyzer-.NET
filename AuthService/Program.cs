using Microsoft.EntityFrameworkCore;
using System.IO;
using AuthService.Models; 

var builder = WebApplication.CreateBuilder(args);


var solutionRoot = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "", "ResumeAnalyzer");
var configPath = Path.Combine(solutionRoot, "appsettings.json");

builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("AuthConnection"); 

builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
