using System;
using Microsoft.AspNetCore.Mvc;
using ApplyService.Models;
using Microsoft.AspNetCore.Authorization;
using ApplyService.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApplyService.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    [Route("api/apply")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserHandler _resumeUploadHandler;

        public UserController(UserHandler resumeUploadHandler)
        {
            _resumeUploadHandler = resumeUploadHandler;
        }

        [HttpPost("resume")]
        public async Task<IActionResult> UploadResume([FromForm] IFormFile file, [FromForm] string name, [FromForm] string email)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required." });
            }

            var result = await _resumeUploadHandler.HandleAsync(file, name, email);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message, userId = result.UserId });
        }
    }
}