using System;
using System.Net.Mail;
using EmailService.Requests;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace EmailService.Handlers
{
	public class EmailServiceHandler
	{
        private readonly IConfiguration _configuration;

        public EmailServiceHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("ResumeAnalyzer", _configuration["EmailSettings:AdminEmail"]));
                email.To.Add(new MailboxAddress("", emailRequest.To));
                email.Subject = emailRequest.Subject;
                email.Body = new TextPart("html") { Text = emailRequest.Body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    MailKit.Security.SecureSocketOptions.StartTls 
                );

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:AdminEmail"],
                    _configuration["EmailSettings:SmtpPassword"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

