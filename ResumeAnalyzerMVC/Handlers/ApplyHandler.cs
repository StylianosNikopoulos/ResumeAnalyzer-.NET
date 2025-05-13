using System;

namespace ResumeAnalyzerMVC.Handlers
{
    public class ApplyHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _applyServiceUrl;

        public ApplyHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _applyServiceUrl = configuration["ApiUrls:APPLY_SERVICE_URL"] ?? throw new ArgumentNullException("ApiUrls:APPLY_SERVICE_URL is missing.");
        }

        public async Task<(bool success, string message, int statusCode)> ApplyAsync(string name, string email, IFormFile file)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(name), "name");
                formData.Add(new StringContent(email), "email");

                if (file != null && file.Length > 0)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    formData.Add(fileContent, "file", file.FileName);
                }

                var response = await _httpClient.PostAsync($"{_applyServiceUrl}/resume", formData);
                return await ParseResponse(response);
            }
        }

        private async Task<(bool success, string message, int statusCode)> ParseResponse(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                string errorResponseContent = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to apply, status: {response.StatusCode} - {errorResponseContent}", statusCode);
            }

            string successResponseContent = await response.Content.ReadAsStringAsync();
            return (true, successResponseContent, statusCode);
        }
    }
}
