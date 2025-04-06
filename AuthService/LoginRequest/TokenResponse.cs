using System;
namespace AuthService.LoginRequest
{
    public class TokenResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}

