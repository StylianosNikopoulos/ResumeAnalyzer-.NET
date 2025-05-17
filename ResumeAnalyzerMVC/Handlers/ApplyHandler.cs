using System;
using NuGet.Common;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using ResumeAnalyzerMVC.Requests;
using ResumeAnalyzerMVC.Responces;

namespace ResumeAnalyzerMVC.Handlers
{
    public class ApplyHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _applyServiceUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplyHandler(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _applyServiceUrl = configuration["ApiUrls:APPLY_SERVICE_URL"] ?? throw new ArgumentNullException("ApiUrls:APPLY_SERVICE_URL is missing.");
        }

        public async Task<ApiResponse<string>> ApplyAsync(UploadResumeRequest uploadRequest)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(uploadRequest.Name), "name");
                formData.Add(new StringContent(uploadRequest.Email), "email");

                if (uploadRequest.File != null && uploadRequest.File.Length > 0)
                {
                    var fileContent = new StreamContent(uploadRequest.File.OpenReadStream());
                    formData.Add(fileContent, "file", uploadRequest.File.FileName);
                }

                var token = _httpContextAccessor.HttpContext?.Session?.GetString("UserToken");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_applyServiceUrl}/resume")
                {
                    Content = formData
                };

                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                return await ParseResponse(response);
            }
        }

        private async Task<ApiResponse<string>> ParseResponse(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;
            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Failed to apply, status: {statusCode} - {responseContent}",
                    Data = null
                };
            }

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Resume uploaded successfully.",
                Data = responseContent
            };
        }
    }
}
