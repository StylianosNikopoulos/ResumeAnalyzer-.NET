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

        public async Task<IActionResult> Index()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            var (success, resumesOrMessage, statusCode) = await _resumesHandler.ShowResumesAsync();

            if (!success)
            {
                string message = resumesOrMessage as string;
                return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = message });
            }

            if (resumesOrMessage is List<UserInfo> resumes)
            {
                return View(resumes);
            }

            return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = "Your custom error message here." });
        }


        public async Task<IActionResult> FilterResumes(List<string> keywords)
        {
            var (success, resumesOrMessage, statusCode) = await _resumesHandler.FilterResumeAsync(keywords);

            if (!success)
            {
                string message = resumesOrMessage as string;
                return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = message });
            }

            if (resumesOrMessage is List<UserInfo> resumes)
            {
                ViewBag.Resumes = resumes.Take(10).ToList(); 
                return View();
            }

            return View("~/Views/Home/Error.cshtml", new ErrorViewModel { Message = "An unexpected error occurred." });
        }

        //public async Task<IActionResult> DownloadResume()
        //{

        //}

    }
}

