using System;
using Microsoft.AspNetCore.Mvc;

namespace ResumeAnalyzerMVC.Controllers
{
    public class ContactFormController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendForm(string name, string email, string message)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

