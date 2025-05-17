using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserService.Requests;

namespace UserService.Controllers
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
        public async Task<IActionResult> UploadResume([FromForm] UserRequest request)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required." });
            }

            if (request.File == null)
            {
                return BadRequest(new { message = "File is required." });
            }

            var response = await _resumeUploadHandler.HandleAsync(request.File, request.Name, request.Email);

            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Message, userId = response.UserId });
        }
    }
}