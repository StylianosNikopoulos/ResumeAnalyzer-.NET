using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuthService.LoginRequest;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Services;
namespace ResumeAnalyzerMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly string _authServiceUrl;
        private readonly AuthServiceDbContext _context;

        public AuthController(ApiService apiService, IConfiguration config, AuthServiceDbContext context)  
        {
            _apiService = apiService;
            _authServiceUrl = config["ApiUrls:AuthService"];
            _context = context;
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
        public async Task<IActionResult> Login(UserLoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                var response = await _apiService.PostAsync($"{_authServiceUrl}/login", model);

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
                {
                    ViewData["ErrorMessage"] = "Invalid credentials!";
                    return View("LoginView");
                }

                HttpContext.Session.SetString("UserToken", tokenResponse.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Login", model);

            }

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserToken");
            return RedirectToAction("Login");
        }
    }
}