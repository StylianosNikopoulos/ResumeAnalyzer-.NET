using System;
using Microsoft.AspNetCore.Mvc;
using ResumeService.Models;
using ApplyService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

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

            var rootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "UserService");
            var filePath = Path.Combine(rootPath, user.ResumePath);

            Console.WriteLine("Expected File Path: " + filePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterResumes([FromBody] List<string> keywords)
        {
            if (keywords == null || !keywords.Any())
                return BadRequest("Keywords cannot be empty.");

            // Fetch all resumes with paths
            var resumes = _usercontext.UserInfos
                .Where(u => !string.IsNullOrEmpty(u.ResumePath))
                .ToList();

            var filteredResumes = new List<UserInfo>();

            foreach (var resume in resumes)
            {
                string pdfPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "UserService", resume.ResumePath);

                if (!System.IO.File.Exists(pdfPath))
                    continue;

                string text = ExtractTextFromPdf(pdfPath);
                if (keywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    filteredResumes.Add(resume);
                }
            }

            return Ok(filteredResumes);
        }

        // Helper Method: Extract Text from PDF
        private string ExtractTextFromPdf(string pdfPath)
        {
            using (var document = PdfDocument.Open(pdfPath))
            {
                var text = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    text.Append(ContentOrderTextExtractor.GetText(page));
                }
                return text.ToString();
            }
        }
    }
}