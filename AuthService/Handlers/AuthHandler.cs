using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Request;
using AuthService.Enum;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Handlers
{
	public class AuthHandler
	{
        private readonly AuthServiceDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthHandler(AuthServiceDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; 
            _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing.");
        }

        public async Task<TokenResponse> RegisterUserAsync(UserRegisterRequest userRegister)
		{
            if (string.IsNullOrEmpty(userRegister.Name) || string.IsNullOrEmpty(userRegister.Password))
                return new TokenResponse { Status = 400, Message = "Name and Password are required." };

            if (await _context.Users.AnyAsync(u => u.Email == userRegister.Email))
                return new TokenResponse { Status = 409, Message = "User with this email already exists." };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == RolesEnum.User.ToString());

            var newUser = new User
            {
                Name = userRegister.Name,
                Email = userRegister.Email,
                PasswordHash = passwordHash,
                RoleId = userRole?.Id ?? 7,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(newUser);
            _httpContextAccessor.HttpContext.Session.SetString("UserToken", token);

            return new TokenResponse {
                Status = 201,
                Message = "Register success",
                Token = token
            };
        }

        public async Task<TokenResponse> LoginUserAsync(UserLoginRequest userLoginRequest)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == userLoginRequest.Email);

            if (user == null)
                return new TokenResponse { Status = 401, Message = "Invalid credentials." };

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(userLoginRequest.Password, user.PasswordHash);

            if (!isPasswordCorrect)
                return new TokenResponse { Status = 401, Message = "Invalid credentials." };

            var token = GenerateJwtToken(user);
            return new TokenResponse
            {
                Status = 200,
                Message = "Login success",
                Token = token
            };

        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
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

