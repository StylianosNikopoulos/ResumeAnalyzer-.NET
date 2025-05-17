using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ResumeAnalyzerMVC.Requests;
using ResumeAnalyzerMVC.Responces;

namespace ResumeAnalyzerMVC.Handlers
{
	public class AuthenticationHandler
	{
		private readonly HttpClient _httpClient;
		private readonly string _authServiceUrl;

        public AuthenticationHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authServiceUrl = configuration["ApiUrls:AUTH_SERVICE_URL"] ?? throw new ArgumentNullException("ApiUrls:AUTH_SERVICE_URL is missing.");
        }

        public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest registerRequest)
		{
            var jsonContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_authServiceUrl}/register", jsonContent);
            return await ParseResponse(response);
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginRequest loginRequest)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_authServiceUrl}/login", jsonContent);
            return await ParseResponse(response);

        }

        private async Task<AuthenticationResponse> ParseResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    string token = jsonResponse.TryGetProperty("token", out var tokenElement) ? tokenElement.GetString() : null;
                    string userName = jsonResponse.TryGetProperty("userName", out var userNameElement) ? userNameElement.GetString() : null;
                    string message = jsonResponse.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : "Login successful";


                    return new AuthenticationResponse
                    {
                        Success = true,
                        Token = token,
                        UserName = userName,
                        Message = message
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing response: " + ex.Message);
                    return new AuthenticationResponse
                    {
                        Success = false,
                        Message = "Failed to parse response."
                    };
                }
            }

            return new AuthenticationResponse
            {
                Success = false,
                Message = responseContent
            };
        }

    }
}

