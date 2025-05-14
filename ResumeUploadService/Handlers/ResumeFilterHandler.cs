using System;
using System.Text;
using ApplyService.Models;
using Microsoft.EntityFrameworkCore;
using ResumesService.Responces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ResumesService.Handlers
{
	public class ResumeFilterHandler
	{
        private readonly UserServiceDbContext _usercontext;
        private readonly IWebHostEnvironment _environment;

        public ResumeFilterHandler(UserServiceDbContext usercontext, IWebHostEnvironment environment)
        {
            _usercontext = usercontext;
            _environment = environment;
        }

        public async Task<ResumeFilterResult> HandleResumesAsync()
		{
            var resumes = await _usercontext.UserInfos.ToListAsync();

            return new ResumeFilterResult
            {
                Success = true,
                Message = "Success handled all Resumes",
                UserInfos = resumes
            };
        }

        public async Task<string?> GetResumePathByUserId(int userId)
        {
            var user = await _usercontext.UserInfos.FindAsync(userId);

            if (user == null || string.IsNullOrEmpty(user.ResumePath))
                return null;

            var rootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "UserService");
            var fullPath = Path.Combine(rootPath, user.ResumePath);

            if (!fullPath.StartsWith(rootPath))
                throw new UnauthorizedAccessException("Invalid file path");

            return System.IO.File.Exists(fullPath) ? fullPath : null;
        }

        public async Task<List<UserInfo>> FilterResumesByKeywordsAsync(List<string> keywords)
        {
            var resumes = _usercontext.UserInfos
                .Where(u => !string.IsNullOrEmpty(u.ResumePath))
                .ToList();

            var filteredResumes = new List<UserInfo>();
            foreach (var resume in resumes)
            {
                var resumePath = Path.Combine(_environment.ContentRootPath, "Resumes", resume.ResumePath);

                if (!System.IO.File.Exists(resumePath))
                    continue;

                string text = ExtractTextFromPdf(resumePath);
                if (keywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    filteredResumes.Add(resume);
                }
            }
            return filteredResumes;
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

