using System;
using AuthService.AuthRequest;
using Microsoft.AspNetCore.Mvc;
using AuthService.Handlers;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AuthHandler _authHandler;

        public AuthController(AuthHandler authHandler)
        {
            _authHandler = authHandler;
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegister)
        {
            var response = await _authHandler.RegisterUserAsync(userRegister);
            return StatusCode(response.Status, response);
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var response = await _authHandler.LoginUserAsync(userLoginRequest);
            return StatusCode(response.Status, response);
        }
    }
}
