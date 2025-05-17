using System;
namespace UserService.Responses
{
    public class UserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
    }
}

