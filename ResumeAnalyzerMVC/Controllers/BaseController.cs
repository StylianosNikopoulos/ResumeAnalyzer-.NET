using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ResumeAnalyzerMVC.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("UserToken");
            ViewBag.auth = !string.IsNullOrEmpty(token);
            ViewBag.role = "User"; 

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                ViewBag.role = roleClaim ?? "User";
            }

            base.OnActionExecuting(context);
        }
    }
}

