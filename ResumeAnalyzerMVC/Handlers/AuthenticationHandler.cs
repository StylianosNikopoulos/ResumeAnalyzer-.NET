using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthService.Enum;
using AuthService.LoginRequest;
using ResumeAnalyzerMVC.Shared;

namespace ResumeAnalyzerMVC.Handlers
{
	public class AuthenticationHandler
	{
		private readonly HttpClient _httpClient;
		private readonly string _authServiceUrl;
        private readonly IConfiguration _configuration;

        public AuthenticationHandler(HttpClient httpClient,IConfiguration configuration)
		{
			_httpClient = httpClient;
            _configuration = configuration;
            _authServiceUrl = _configuration["AUTH_SERVICE_URL"] ?? "http://localhost:5013/api/auth"; 
        }

        public async Task<(bool success,string token,string message)> RegisterAsync(string name,string email,string password)
		{
			var regModel = new
			{
				Name = name,
				Email = email,
                PasswordHash = HashPassword(password),
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(regModel), Encoding.UTF8, "application/json");

			try
			{
				var response = await _httpClient.PostAsync($"{_authServiceUrl}/register",jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (false,null,"Please put correct pass or name");
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                //var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Token))
                {
                    return (true, apiResponse.Token, "User registered successfully");
                }
                else
                {
                    return (false,null, apiResponse?.Message ?? "error");
                }

            }
            catch (Exception ex)
			{
                return (false,null, ex.Message);
            }
        }

        public async Task<(bool success,string token, string message)> LoginAsync(string email,string password)
        {
            var loginModel = new
            {
                Email = email,
                Password = password,  
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_authServiceUrl}/login", jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, "Invalid credentials");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    return (true, tokenResponse.Token, "Login successful");
                }
                else
                {
                    return (false, null, "Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                return (false, null, "Please try again later");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
    
    public class TokenResponse
    {
        public string Token { get; set; }
    }
}

