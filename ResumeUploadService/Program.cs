using Microsoft.EntityFrameworkCore;
using ResumeService.Models;
using ApplyService.Models;
using ResumesService.Handlers;

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "", "ResumeAnalyzer");
var configPath = Path.Combine(solutionRoot, "appsettings.json");
builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);


builder.Services.AddDbContext<ResumeAnalyzerDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ResumeConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ResumeConnection"))));


builder.Services.AddDbContext<UserServiceDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ApplyConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ApplyConnection"))));

builder.Services.AddScoped<ResumeFilterHandler>();

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
