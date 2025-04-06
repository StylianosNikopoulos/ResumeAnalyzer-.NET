using AuthService.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
Env.Load();


//builder.Configuration.AddJsonFile("appsettings.authservice.json", optional: false, reloadOnChange: true);
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Fallback: Build the connection string manually if not loaded correctly
if (string.IsNullOrEmpty(connectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    var dbName = Environment.GetEnvironmentVariable("AUTH_DB");
    var dbUser = Environment.GetEnvironmentVariable("MYSQL_USER");
    var dbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");


    connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
}
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is missing! Check your .env file.");
}



builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuthServiceDbContext>();

var app = builder.Build();


// Configure the HTTP request pipeline.
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

