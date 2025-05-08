using System;
using System.Text;
using System.Text.Json;
using ApplyService.Models;

namespace ResumeAnalyzerMVC.Handlers
{
    public class ResumesHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _resumesService;   

        public ResumesHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _resumesService = configuration["ApiUrls:AUTH_SERVICE_URL"] ?? "https://localhost:7083/api/resumes";
        }

        public async Task<(bool success, object resumesOrMessage, int statusCode)> ShowResumesAsync()
        {
            var response = await _httpClient.GetAsync($"{_resumesService}/resumes");

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                return (false, errorMessage, (int)response.StatusCode);
            }

            var resumes = JsonSerializer.Deserialize<List<UserInfo>>(await response.Content.ReadAsStringAsync());
            return (true, resumes, (int)response.StatusCode);
        }


        public async Task<(bool success, string message, int statusCode)> DownloadResumeAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_resumesService}/download-resume/{id}");

            return await ParseResponse(response);
        }

        public async Task<(bool success, object resumesOrMessage, int statusCode)> FilterResumeAsync(List<string> keywords)
        {
            var url = $"{_resumesService}/filter";
            var content = new StringContent(JsonSerializer.Serialize(keywords), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var resumes = JsonSerializer.Deserialize<List<UserInfo>>(await response.Content.ReadAsStringAsync());
                return (true, resumes, (int)response.StatusCode); 
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage, (int)response.StatusCode); 
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