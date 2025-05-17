using System;
using System.Text;
using System.Text.Json;
using ResumeAnalyzerMVC.Requests;
using ResumeAnalyzerMVC.Responses;

namespace ResumeAnalyzerMVC.Handlers
{
	public class ContactFormHandler
	{
        private readonly HttpClient _httpClient;
        private readonly string _emailServiceUrl;
        private readonly IConfiguration _configuration;

        public ContactFormHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _emailServiceUrl = configuration["ApiUrls:EMAIL_SERVICE_URL"] ?? throw new ArgumentNullException("ApiUrls:EMAIL_SERVICE_URL is missing.");
        }

        public async Task<ContactFormResponse> SendContactFormAsync(ContactFormRequest contactRequest)
        {
            var emailRequest = new
            {
                To = _configuration["EmailSettings:AdminEmail"],
                Subject = $"Contact Form Message from {contactRequest.Name}",
                Body = $"Name: {contactRequest.Name}\nEmail: {contactRequest.Email}\nMessage: {contactRequest.Message}"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonContent = JsonSerializer.Serialize(emailRequest, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_emailServiceUrl}/api/email/send", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new ContactFormResponse
            {
                Success = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? "Message sent successfully." : $"Failed to send message: {responseContent}"
            };
        }
    }
}

