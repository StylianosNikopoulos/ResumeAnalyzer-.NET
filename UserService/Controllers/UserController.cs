using System;
using Microsoft.AspNetCore.Mvc;
using ApplyService.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApplyService.Controllers
{
    [Route("api/apply")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserServiceDbContext _context;

        public UserController(UserServiceDbContext context)
        {
            _context = context;
        }

        [HttpPost("resume")]
        //[Authorize]
        public async Task<IActionResult> UploadResume([FromForm] IFormFile file, [FromForm] string name, [FromForm] string email)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var filePath = Path.Combine("Resumes", $"{Guid.NewGuid()}_{file.FileName}");
            Directory.CreateDirectory("Resumes");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newUserInfo = new UserInfo
            {
                Name = name,
                Email = email,
                ResumePath = filePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserInfos.Add(newUserInfo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Resume uploaded successfully", userId = newUserInfo.Id });
        }
    }
}