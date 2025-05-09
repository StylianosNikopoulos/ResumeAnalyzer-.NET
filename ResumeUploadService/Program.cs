using Microsoft.EntityFrameworkCore;
using ResumeService.Models;
using ApplyService.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var configPath = "/Users/a/PROJECTS/ResumeAnalyzer/appsettings.json";
builder.Configuration.SetBasePath(Path.GetDirectoryName(configPath)!)
                      .AddJsonFile(Path.GetFileName(configPath), optional: false, reloadOnChange: true);


builder.Services.AddDbContext<ResumeAnalyzerDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ResumeConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ResumeConnection"))));


builder.Services.AddDbContext<UserServiceDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ApplyConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ApplyConnection"))));

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
