using System;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Requests;

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
        public async Task<IActionResult> UploadResume(UploadResumeRequest uploadRequest)
        {
            var token = HttpContext.Session.GetString("UserToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Authentication"); 
            }

            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
            {
                TempData["ErrorMessage"] = "No file selected.";
                return RedirectToAction("Index");
            }

            var response = await _applyHandler.ApplyAsync(uploadRequest);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Your resume was uploaded successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}

