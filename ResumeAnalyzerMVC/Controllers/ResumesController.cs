using System;
using ApplyService.Models;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ResumeAnalyzerMVC.Controllers
{
    public class ResumesController : Controller
    {
        private readonly Handlers.ResumesHandler _resumesHandler;

        public ResumesController(ResumesHandler resumesHandler)
        {
            _resumesHandler = resumesHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));

            var (success, resumesOrMessage, statusCode) = await _resumesHandler.ShowResumesAsync();

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to load resumes.";
                return View();
            }

            var resumes = resumesOrMessage as List<UserInfo>;
            return View(resumes);
        }


        [HttpGet]
        public async Task<IActionResult> DownloadResume(int id)
        {
            var (success, fileBytes, fileName, statusCode) = await _resumesHandler.DownloadResumeAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to download resume.";
                return View();
            }

            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> FilterResumes(List<string> keywords)
        {
            var (success, resumesOrMessage, statusCode) = await _resumesHandler.FilterResumeAsync(keywords);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to filter resumes.";
                return View();
            }

            if (resumesOrMessage is List<UserInfo> filteredResumes)
            {
                return View("Index", filteredResumes);
            }

            TempData["ErrorMessage"] = "Failed to filter resumes.";
            return View();
        }
    }
}

