using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuthService.LoginRequest;
using AuthService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzerMVC.Handlers;
using ResumeAnalyzerMVC.Services;
namespace ResumeAnalyzerMVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly Handlers.AuthenticationHandler _authHandler;
        public AuthenticationController(AuthenticationHandler authHandler)  
        {
            _authHandler = authHandler;
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

            var (success, message) = await _authHandler.RegisterAsync(name, email, password);
            if (!success)
            {
                ViewData["ErrorMessage"] = message;
                return View();
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var (success, token, message) = await _authHandler.LoginAsync(email, password);

            if (!success)
            {
                ViewData["ErrorMessage"] = message;
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