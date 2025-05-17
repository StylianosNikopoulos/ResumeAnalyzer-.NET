using System;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using UserService.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ResumeAnalyzerMVC.Controllers
{
    public class ResumesController : BaseController
    {
        private readonly Handlers.ResumesHandler _resumesHandler;

        public ResumesController(ResumesHandler resumesHandler)
        {
            _resumesHandler = resumesHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keywords)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Authentication");

            List<UserInfo> resumes;

            if (!string.IsNullOrEmpty(keywords))
            {
                var keywordList = keywords.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrEmpty(k)).ToList();
                var (success, resumesOrMessage, statusCode) = await _resumesHandler.FilterResumeAsync(keywordList, token);

                if (!success)
                {
                    TempData["ErrorMessage"] = "Failed to filter resumes.";
                    return View(new List<UserInfo>());
                }

                resumes = resumesOrMessage as List<UserInfo> ?? new List<UserInfo>();
            }
            else
            {
                var (success, resumesOrMessage, statusCode) = await _resumesHandler.ShowResumesAsync(token);

                if (!success)
                {
                    TempData["ErrorMessage"] = "Failed to load resumes.";
                    return View(new List<UserInfo>());
                }

                resumes = resumesOrMessage as List<UserInfo> ?? new List<UserInfo>();
            }

            return View(resumes);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadResume(int id)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Authentication");

            var (success, fileBytes, fileName, statusCode) = await _resumesHandler.DownloadResumeAsync(id, token);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to download resume.";
                return View();
            }

            return File(fileBytes, "application/pdf", fileName);
        }


        [HttpPost]
        public async Task<IActionResult> FilterResumes([FromBody] List<string> keywords)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (keywords == null || !keywords.Any())
                return BadRequest("Keywords cannot be empty.");

            var (success, resumesOrMessage, statusCode) = await _resumesHandler.FilterResumeAsync(keywords, token);

            if (!success)
            {
                return StatusCode(statusCode, new { message = resumesOrMessage });
            }

            if (resumesOrMessage is List<UserInfo> filteredResumes)
            {
                return Ok(filteredResumes);
            }

            return StatusCode(statusCode, new { message = "Failed to filter resumes." });
        }
    }
}


