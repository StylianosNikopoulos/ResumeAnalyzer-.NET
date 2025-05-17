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
                return BadRequest(new EmailResponse<object>
                {
                    StatusCode = 400,
                    Message = "Request body cannot be null."
                });

            if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Body))
                return BadRequest(new EmailResponse<object>
                {
                    StatusCode = 400,
                    Message = "Recipient and Body fields are required."
                });

            if (string.IsNullOrEmpty(emailRequest.Subject))
            {
                return BadRequest(new EmailResponse<object>
                {
                    StatusCode = 400,
                    Message = "Subject cannot be empty."
                });
            }

            var success = await _emailServiceHandler.SendEmailAsync(emailRequest);

            if (success)
            {
                return Ok(new EmailResponse<object>
                {
                    StatusCode = 200,
                    Message = "Email sent successfully."
                });
            }

            return StatusCode(500, new EmailResponse<object>
            {
                StatusCode = 500,
                Message = "Failed to send email due to internal server error."
            });
        }
    }
}


