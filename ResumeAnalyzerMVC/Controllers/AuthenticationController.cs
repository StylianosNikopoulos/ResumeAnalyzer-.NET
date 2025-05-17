using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Requests;

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
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                ViewData["ErrorMessage"] = "Passwords do not match!";
                return View();
            }

            var response = await _authHandler.RegisterAsync(registerRequest);
            if (!response.Success)
            {
                ViewData["ErrorMessage"] = "Some error occured";
                return View();
            }
            HttpContext.Session.SetString("UserToken", response.Token);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var response = await _authHandler.LoginAsync(loginRequest);

            if (!response.Success)
            {
                ViewData["ErrorMessage"] = "Wrong Credentials";
                return View("Login");
            }

            HttpContext.Session.SetString("UserToken", response.Token);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserToken");
            return RedirectToAction("Login");
        }
    }
}