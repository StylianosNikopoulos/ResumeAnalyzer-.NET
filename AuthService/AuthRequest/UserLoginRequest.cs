using System;
using AuthService.Enum;

namespace AuthService.AuthRequest
{
    public class UserLoginRequest
    {
        public string Email { get; set; }  
        public string Password { get; set; }
    }
}

