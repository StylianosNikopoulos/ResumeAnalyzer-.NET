using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Enum;
using AuthService.LoginRequest;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AuthServiceDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public AuthController(AuthServiceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing.");
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegister)
        {
            if (string.IsNullOrEmpty(userRegister.Name) || string.IsNullOrEmpty(userRegister.Password))
                return BadRequest("Name and Password are required.");

            if (await _context.Users.AnyAsync(u => u.Email == userRegister.Email))
                return Conflict("User with this email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == RolesEnum.Admin.ToString());
            var newUser = new User {
                Name = userRegister.Name,
                Email = userRegister.Email,
                PasswordHash = passwordHash,
                RoleId = userRole?.Id ?? 1,
                CreatedAt = DateTime.UtcNow };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register success" });
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == userLoginRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginRequest.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");


            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_secretKey);  

            var claims = new[]
            {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role.RoleName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
