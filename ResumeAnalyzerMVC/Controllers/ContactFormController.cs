using System;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Requests;

namespace ResumeAnalyzerMVC.Controllers
{
    public class ContactFormController : BaseController
    {
        private readonly ContactFormHandler _contactFormHandler;

        public ContactFormController(ContactFormHandler contactFormHandler)
        {
            _contactFormHandler = contactFormHandler;
        }

        public IActionResult Index()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendForm(ContactFormRequest contactRequest)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the highlighted errors.";
                return View("Index", contactRequest);
            }

            var response = await _contactFormHandler.SendContactFormAsync(contactRequest);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View("Index", contactRequest);
            }
            else
            {
                TempData["SuccessMessage"] = "Your message has been sent successfully.";
            }

            TempData["SuccessMessage"] = "Your message has been sent successfully.";
            return RedirectToAction("Index");
        }
    }
}