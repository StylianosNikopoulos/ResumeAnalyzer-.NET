using System;
namespace AuthService.LoginRequest
{
    public class UserLoginRequest
    {
        public string Email { get; set; }  
        public string Password { get; set; }  
    }
    public class TokenResponse
    {
        public string Token { get; set; }
    }
}

