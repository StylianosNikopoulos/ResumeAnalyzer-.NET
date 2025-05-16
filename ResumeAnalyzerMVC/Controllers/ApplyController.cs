using System;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;

namespace ResumeAnalyzerMVC.Controllers
{
    public class ApplyController : BaseController
    {
		private readonly ApplyHandler _applyHandler;

		public ApplyController(ApplyHandler applyHandler)
		{
			_applyHandler = applyHandler;
		}

        public IActionResult Index()
		{
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Authentication"); 
            }

            ViewBag.auth = true;
            return View();
        }

		[HttpPost]
        public async Task<IActionResult> UploadResume(string name, string email, IFormFile file)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Authentication"); 
            }

            if (file == null || file.Length == 0)
            {
                ViewData["ErrorMessage"] = "No file selected.";
                return View("Index");
            }

            var (success, message, statusCode) = await _applyHandler.ApplyAsync(name, email, file);

            if (!success)
            {
                ViewData["ErrorMessage"] = "An error occurred while uploading the resume.";
                return View("Index");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

