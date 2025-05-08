using System;
using Microsoft.AspNetCore.Mvc;
using ResumeService.Models;
using ApplyService.Models;
namespace ResumesService.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class ResumeFilterController : Controller
    {
        private readonly ResumeAnalyzerDbContext _context;
        private readonly UserServiceDbContext _usercontext;

        public ResumeFilterController(ResumeAnalyzerDbContext context, UserServiceDbContext usercontext)
        {
            _context = context;
            _usercontext = usercontext;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterResumes([FromBody] List<string> keywords)
        {
            var filterResumes = _context.ResumeKeywords
                       .Where(rk => keywords.Contains(rk.Keyword.Keyword1))
                       .Select(rk => rk.Resume)
                       .Distinct()
                       .ToList();
            return Ok(filterResumes);
        }

        [HttpGet("resumes")]
        public IActionResult GetResumes()
        {
            var resumes = _usercontext.UserInfos  
                .ToList();

            return Ok(resumes);
        }

        [HttpGet("download-resume/{userId}")]
        public IActionResult DownloadResume(int UserId)
        {
            var user = _usercontext.UserInfos.Find(UserId);
            if (user == null || string.IsNullOrEmpty(user.ResumePath))
                return NotFound("Resume not found");

            var filePath = user.ResumePath;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }
    }
}