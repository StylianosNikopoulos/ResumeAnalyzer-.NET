using System;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;

namespace ResumeAnalyzerMVC.Controllers
{
    //[Authorize]
    public class ApplyController : Controller
	{
		private readonly ApplyHandler _applyHandler;

		public ApplyController(ApplyHandler applyHandler)
		{
			_applyHandler = applyHandler;
		}

        public IActionResult Index()
		{
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            return View();
		}

		[HttpPost]
		public async Task<IActionResult> UploadResume(string name, string email, IFormFile file)
		{
            if (file == null || file.Length == 0)
            {
                ViewData["ErrorMessage"] = "No file selected.";
                return View("Index");
            }

            var (success, message, statusCode) = await _applyHandler.ApplyAsync(name, email, file);

            if (!success)
			{
                ViewData["ErrorMessage"] = message;
                return View("Index");
            }
			return RedirectToAction("Index", "Home");
        }
    }
}

