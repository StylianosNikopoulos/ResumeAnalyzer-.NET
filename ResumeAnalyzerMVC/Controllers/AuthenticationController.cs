using System;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
namespace ResumeAnalyzerMVC.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly Handlers.AuthenticationHandler _authHandler;

        public AuthenticationController(AuthenticationHandler authHandler)  
        {
            _authHandler = authHandler;
        }

        public IActionResult Login()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            return View();
        }
        public IActionResult Register()
        {
            ViewBag.auth = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserToken"));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewData["ErrorMessage"] = "Passwords do not match!";
                return View();
            }

            var (success,token, message) = await _authHandler.RegisterAsync(name, email, password);
            if (!success)
            {
                ViewData["ErrorMessage"] = "Some error occured";
                return View();
            }
            HttpContext.Session.SetString("UserToken", token);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var (success, token, message) = await _authHandler.LoginAsync(email, password);

            if (!success)
            {
                ViewData["ErrorMessage"] = "Wrong Credentials";
                return View("Login");
            }

            HttpContext.Session.SetString("UserToken", token);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserToken");
            return RedirectToAction("Login");
        }
    }
}