using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadService.Models;

namespace ResumeAnalyzerService.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class ResumeFilterController : Controller
    {
        private readonly ResumeAnalyzerDbContext _context;

        public ResumeFilterController(ResumeAnalyzerDbContext context)
        {
            _context = context;
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

    }
}