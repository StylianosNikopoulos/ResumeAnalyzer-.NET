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
            _resumesService = configuration["ApiUrls:RESUME_SERVICE_URL"] ?? "https://localhost:7083/api/resumes";
        }

        //Get resumes
        public async Task<(bool success, object resumesOrMessage, int statusCode)> ShowResumesAsync()
        {
            var response = await _httpClient.GetAsync($"{_resumesService}/resumes");
            return await HandleApiResponse<List<UserInfo>>(response);
        }

        //Get resume (download)
        public async Task<(bool success, byte[] fileBytes, string fileName, int statusCode)> DownloadResumeAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_resumesService}/download-resume/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    string fileName = $"Resume_{id}.pdf";

                    return (true, fileBytes, fileName, (int)response.StatusCode);
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                return (false, null, errorMessage, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while downloading resume: {ex.Message}");
                return (false, null, ex.Message, 500);
            }

        }

        // Post (filter base keywords)
        public async Task<(bool success, object resumesOrMessage, int statusCode)> FilterResumeAsync(List<string> keywords)
        {
            var url = $"{_resumesService}/filter";
            var content = new StringContent(JsonSerializer.Serialize(keywords), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await HandleApiResponse<List<UserInfo>>(response);
        }

        //Helper Method
        private async Task<(bool success, object result, int statusCode)> HandleApiResponse<T>(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true 
                    };

                    var result = JsonSerializer.Deserialize<T>(responseData);
                    return (true, result ?? new object(), statusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Deserialization error: " + ex.Message);
                    return (false, "Failed to parse response.", statusCode);
                }
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage, statusCode);
        }
    }
}