﻿using System;
using AuthService.Enum;

namespace AuthService.Request
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  
        public RolesEnum Role { get; set; } 
    }
}

