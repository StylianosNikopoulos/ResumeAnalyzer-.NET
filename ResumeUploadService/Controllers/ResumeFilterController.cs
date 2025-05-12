using System;
using Microsoft.AspNetCore.Mvc;
using ResumesService.Handlers;

namespace ResumesService.Controllers
{
    [Route("api/resumes")]
    [ApiController]
    public class ResumeFilterController : Controller
    {
        private readonly ResumeFilterHandler _resumeFilterHandler;

        public ResumeFilterController(ResumeFilterHandler resumeFilterHandler)
        {
            _resumeFilterHandler = resumeFilterHandler;
        }

        [HttpGet("resumes")]
        public async Task<IActionResult> GetResumes()
        {
            var result = await _resumeFilterHandler.HandleResumesAsync();

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message, resumes = result.UserInfos });
        }

        [HttpGet("download-resume/{userId}")]
        public async Task<IActionResult> DownloadResume(int UserId)
        {
            var resumePath = await _resumeFilterHandler.GetResumePathByUserId(UserId);

            if (resumePath == null)
                return NotFound("Resume not found");

            var fileBytes = System.IO.File.ReadAllBytes(resumePath);
            var fileName = Path.GetFileName(resumePath);
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterResumes([FromBody] List<string> keywords)
        {
            if (keywords == null || !keywords.Any())
                return BadRequest("Keywords cannot be empty.");

            var filteredResumes = await _resumeFilterHandler.FilterResumesByKeywordsAsync(keywords);
            return Ok(filteredResumes);
        }
    }
}