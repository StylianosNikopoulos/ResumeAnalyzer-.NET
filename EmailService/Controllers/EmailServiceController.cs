using System;
using EmailService.Handlers;
using EmailService.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailServiceController : Controller
	{
        private readonly EmailServiceHandler _emailServiceHandler;

        public EmailServiceController(EmailServiceHandler emailServiceHandler)
		{
            _emailServiceHandler = emailServiceHandler;
		}


        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if(string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Body))
            {
                return BadRequest("Recipient and body cannot be empty.");
            }

            var success = await _emailServiceHandler.SendEmailAsync(emailRequest);

            if (success)
                return Ok(new { message = "Email sent successfully." });

            return StatusCode(500, new { message = "Failed to send email." });

        }
    }
}

