using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthService.Enum;
using AuthService.LoginRequest;

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

            // Log the full response content
            Console.WriteLine("API Response Content: " + responseContent);
            Console.WriteLine("Status Code: " + response.StatusCode);

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    // Handle non-success status codes (e.g., 400, 500)
                    return (false, null, $"Error: {response.StatusCode}, Message: {responseContent}");
                }

                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (jsonResponse.TryGetProperty("token", out var tokenProperty))
                {
                    string token = tokenProperty.GetString();
                    return (true, token, "Success");
                }
                else
                {
                    return (false, null, "Token not found in the response.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing response: {ex.Message}");
                return (false, null, "Failed to parse response.");
            }
        }

    }
}

