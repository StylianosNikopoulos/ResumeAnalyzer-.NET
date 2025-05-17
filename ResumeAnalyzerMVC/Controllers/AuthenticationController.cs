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
                TempData["ErrorMessage"] = "Passwords do not match. Please try again.";
                return RedirectToAction("Register");
            }

            var response = await _authHandler.RegisterAsync(registerRequest);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = "Registration failed. Please try again.";
                return RedirectToAction("Register");
            }

            HttpContext.Session.SetString("UserToken", response.Token);
            TempData["SuccessMessage"] = "Registration successful! You are now logged in.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var response = await _authHandler.LoginAsync(loginRequest);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return View("Login");
            }

            HttpContext.Session.SetString("UserToken", response.Token);
            TempData["SuccessMessage"] = "Login successful! Welcome back.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserToken");
            TempData["SuccessMessage"] = "You have successfully logged out.";
            return RedirectToAction("Login");
        }
    }
}