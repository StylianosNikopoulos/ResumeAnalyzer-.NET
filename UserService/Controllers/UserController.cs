using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserServiceDbContext _context;

        public UserController(UserServiceDbContext context)
        {
            _context = context;
        }


        [HttpPost("upload-resume")]
        public async Task<IActionResult> UploadResume([FromForm] IFormFile file, [FromForm] int userId)
        {
            var files = Request.Form.Files;

            if (files.Count == 0)
                return BadRequest("No files received.");

            var resume = files[0]; 

            if (resume == null || resume.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            var filePath = Path.Combine("Resumes", $"{Guid.NewGuid()}_{resume.FileName}");
            Directory.CreateDirectory("Resumes"); 

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }

            var user = await _context.UserInfos.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.ResumePath = filePath;
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("resumes")]
        public IActionResult GetResumes()
        {
            var resumes = _context.UserInfos   //Select
                .ToList();

            return Ok(resumes);
        }

        [HttpGet("download-resume/{userId}")]
        public IActionResult DownloadResume(int UserId)
        {
            var user = _context.UserInfos.Find(UserId);
            if(user == null || string.IsNullOrEmpty(user.ResumePath))
            {
                return NotFound("Resume not found");
            }

            var filePath = user.ResumePath;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }
    }
}