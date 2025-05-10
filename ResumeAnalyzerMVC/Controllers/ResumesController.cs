using System;
using ApplyService.Models;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Models;
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

            if (success)
            {
                return File(fileBytes, "application/pdf", fileName);
            }

            return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = fileName });
        }

        [HttpPost]
        public async Task<IActionResult> FilterResumes(List<string> keywords)
        {
            var (success, resumesOrMessage, statusCode) = await _resumesHandler.FilterResumeAsync(keywords);

            if (!success)
            {
                return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = resumesOrMessage.ToString() });
            }

            if (resumesOrMessage is List<UserInfo> filteredResumes)
            {
                return View("Index", filteredResumes);
            }

            return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = "An unexpected error occurred." });
        }
    }
}

