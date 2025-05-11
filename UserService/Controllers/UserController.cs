using System;
using Microsoft.AspNetCore.Mvc;
using ApplyService.Models;
using Microsoft.AspNetCore.Authorization;
using ApplyService.Handlers;

namespace ApplyService.Controllers
{
    [Route("api/apply")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplyHandler _resumeUploadHandler;

        public UserController(ApplyHandler resumeUploadHandler)
        {
            _resumeUploadHandler = resumeUploadHandler;
        }

        [HttpPost("resume")]
        //[Authorize]
        public async Task<IActionResult> UploadResume([FromForm] IFormFile file, [FromForm] string name, [FromForm] string email)
        {
            var result = await _resumeUploadHandler.HandleAsync(file, name, email);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message, userId = result.UserId });
        }
    }
}