using System;
using System.Text;
using System.Text.Json;

namespace ResumeAnalyzerMVC.Services
{
	public class ApiService
	{
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> PostAsync(string url, object data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}

