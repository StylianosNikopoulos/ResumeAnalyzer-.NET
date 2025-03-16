using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Services;

namespace ResumeAnalyzerMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly string _authServiceUrl;

        public AuthController(ApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _authServiceUrl = config["ApiUrls:AuthService"];
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
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

            var registerModel = new {Name = name, Email = email, PasswordHash = password, Role = "User" };
            var response = await _apiService.PostAsync($"{_authServiceUrl}/register", registerModel);

            if (response.Contains("Error"))
            {
                ViewData["ErrorMessage"] = "Registration failed!";
                return View();
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var response = await _apiService.PostAsync($"{_authServiceUrl}/login", new { Email = email, Password = password });

            if (response.Contains("Unauthorized"))
            {
                ViewData["ErrorMessage"] = "Invalid credentials!";
                return View();
            }

            HttpContext.Session.SetString("UserToken", response);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserToken");
            return RedirectToAction("Login");
        }
    }
}