using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Enum;
using AuthService.LoginRequest;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            //_secretKey = "V4X9ed4Jj74yG3B6SLquDsOmTRlxZqv6beO7OH3JW2Q=";
            _secretKey = _configuration["JWT_SECRET"];
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegister)
        {
            if (userRegister == null)
            {
                return BadRequest("Invalid user data");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userRegister.Email);

            if (existingUser != null)
            {
                return Conflict("User with this email already exists");
            }

            var password = HashPassword(userRegister.PasswordHash);

            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == RolesEnum.User.ToString());

            if (userRole == null)
            {
                return BadRequest("Invalid role specified.");
            }

            var newUser = new User
            {
                Name = userRegister.Name,
                Email = userRegister.Email,
                PasswordHash = password,
                RoleId = userRole.Id,  
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            if (userLoginRequest == null || string.IsNullOrEmpty(userLoginRequest.Email) || string.IsNullOrEmpty(userLoginRequest.Password))
            {
                return BadRequest(new { status = "error", message = "Invalid login credentials" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == userLoginRequest.Email);

            if (user == null)
            {
                return Unauthorized(new { status = "error", message = "User not found" });
            }

            if (!VerifyPassword(userLoginRequest.Password, user.PasswordHash))
            {
                return Unauthorized(new { status = "error", message = "Incorrect password" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { status = "success", token = token });
        }


        private string GenerateJwtToken(User user)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new ArgumentNullException("JwtSettings:SecretKey", "Secret key is not configured in appsettings.json.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);  // Convert the secret key to bytes

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

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);  // Return the generated token
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                var hashedPassword = Convert.ToBase64String(hashedBytes);
                return hashedPassword == storedPasswordHash;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
