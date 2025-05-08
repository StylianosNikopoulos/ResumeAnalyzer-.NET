using System;
using System.Text;
using System.Text.Json;

namespace ResumeAnalyzerMVC.Handlers
{
	public class AuthenticationHandler
	{
		private readonly HttpClient _httpClient;
		private readonly string _authServiceUrl;

        public AuthenticationHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authServiceUrl = configuration["ApiUrls:AUTH_SERVICE_URL"] ?? "https://localhost:7144/api/auth";
        }

        public async Task<(bool success,string token,string message)> RegisterAsync(string name,string email,string password)
		{
            var jsonContent = new StringContent(JsonSerializer.Serialize(new { name, email, password }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_authServiceUrl}/register", jsonContent);
            return await ParseResponse(response);
        }

        public async Task<(bool success,string token, string message)> LoginAsync(string email,string password)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_authServiceUrl}/login", jsonContent);
            return await ParseResponse(response);

        }

        private async Task<(bool success, string token, string message)> ParseResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    string token = jsonResponse.TryGetProperty("token", out var tokenElement) ? tokenElement.GetString() : null;
                    string message = jsonResponse.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : "No message";

                    return (true, token, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing response: " + ex.Message);
                    return (false, null, "Failed to parse response.");
                }
            }

            return (false, null, "Failed");
        }

    }
}

