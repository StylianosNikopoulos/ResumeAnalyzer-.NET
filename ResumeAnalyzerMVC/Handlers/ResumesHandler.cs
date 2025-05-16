using System;
using System.Text;
using System.Text.Json;
using UserService.Models;
using ResumeAnalyzerMVC.Responces;

namespace ResumeAnalyzerMVC.Handlers
{
    public class ResumesHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _resumesService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResumesHandler(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _resumesService = configuration["ApiUrls:RESUME_SERVICE_URL"] ?? throw new ArgumentNullException("ApiUrls:RESUME_SERVICE_URL is missing.");
        }

        // Get resumes 
        public async Task<(bool success, object resumesOrMessage, int statusCode)> ShowResumesAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_resumesService}/resumes");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            return await HandleApiResponseForResumes<List<UserInfo>>(response);
        }

        public async Task<(bool success, object resumesOrMessage, int statusCode)> FilterResumeAsync(List<string> keywords, string token)
        {
            var url = $"{_resumesService}/filter";
            var content = new StringContent(JsonSerializer.Serialize(keywords), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            return await HandleApiResponse<List<UserInfo>>(response);
        }

        public async Task<(bool success, byte[] fileBytes, string fileName, int statusCode)> DownloadResumeAsync(int id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_resumesService}/download-resume/{id}");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                string fileName = $"Resume_{id}.pdf";
                return (true, fileBytes, fileName, (int)response.StatusCode);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, null, errorMessage, (int)response.StatusCode);
            }
        }




        //Helper for ShowResumesAsync
        private async Task<(bool success, object result, int statusCode)> HandleApiResponseForResumes<T>(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;
            var responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var apiResponse = JsonSerializer.Deserialize<ApiResponseForResumes<T>>(responseData, options);

                    if (apiResponse != null && apiResponse.Resumes != null)
                    {
                        return (true, apiResponse.Resumes, statusCode);
                    }

                    return (false, "No resumes found or failed to parse the data.", statusCode);
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

        // Helper for FilterResumeAsync
        private async Task<(bool success, object result, int statusCode)> HandleApiResponse<T>(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;
            var responseData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (responseData.TrimStart().StartsWith("["))
                    {
                        var plainList = JsonSerializer.Deserialize<T>(responseData, options);
                        return (true, plainList, statusCode);
                    }

                    var apiResponse = JsonSerializer.Deserialize<ApiResponseForFilterMethod<T>>(responseData, options);
                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        return (true, apiResponse.Data, statusCode);
                    }

                    return (false, "No data found or failed to parse the data.", statusCode);
                }
                catch (JsonException jsonEx)
                {
                    return (false, $"Failed to parse response due to invalid JSON. Error: {jsonEx.Message}", statusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                }
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            return (false, errorMessage, statusCode);
        }
    }
}