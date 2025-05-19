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
            try
            {
                var emailRequest = new
                {
                    To = _configuration["EmailSettings:AdminEmail"] ?? "[No Admin Email]",
                    Subject = $"Contact Form Message from {contactRequest.Name ?? "[No Name]"}",
                    Body = $"Name: {contactRequest.Name ?? "[No Name]"}\nEmail: {contactRequest.Email ?? "[No Email]"}\nMessage: {contactRequest.Message ?? "[No Message]"}"
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = JsonSerializer.Serialize(emailRequest, jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_emailServiceUrl}/send", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {responseContent}");

                return new ContactFormResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Message sent successfully." : $"Failed to send message: {responseContent}"
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return new ContactFormResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
    }
}

