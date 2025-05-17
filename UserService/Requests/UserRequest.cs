using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.Requests
{
    public class UserRequest
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

