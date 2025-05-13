using Microsoft.EntityFrameworkCore;
using ApplyService.Models;
using ApplyService.Handlers;

var builder = WebApplication.CreateBuilder(args);

var solutionRoot = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "", "ResumeAnalyzer");
var configPath = Path.Combine(solutionRoot, "appsettings.json");
builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

builder.Services.AddDbContext<UserServiceDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ApplyConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ApplyConnection"))));



//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false,
//            ValidateIssuerSigningKey = false,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
//        };
//    });

builder.Services.AddScoped<ApplyHandler>();

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
