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

        public async Task<string> PostAsync<T>(string url, T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: {response.StatusCode}, Message: {result}");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while making the POST request.", ex);
            }
        }

    }
}

