using System;
using EmailService.Handlers;
using EmailService.Requests;
using EmailService.Responses;
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
            if (emailRequest == null)
                return BadRequest(new EmailResponse<object>(400, "Request body cannot be null."));

            if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Body))
                return BadRequest(new EmailResponse<object>(400, "Recipient and body are required."));

            var success = await _emailServiceHandler.SendEmailAsync(emailRequest);

            if (success)
                return Ok(new EmailResponse<object>(200, "Email sent successfully."));

            return StatusCode(500, new EmailResponse<object>(500, "Failed to send email."));
        }
    }
}

