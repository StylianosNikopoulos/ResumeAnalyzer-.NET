using System;
namespace AuthService.AuthRequest
{
    public class TokenResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}

