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
                    return (false,null, "Unable to register");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    return (true, tokenResponse.Token, "User registered successfully");
                }
            }
            catch (Exception ex)
			{
                Console.WriteLine(ex);
            }
            return (false, null, "Error during registration");
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

                var tokenResponse = JsonSerializer.Deserialize<AuthService.LoginRequest.TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    return (true, tokenResponse.Token, "Login successful");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return (false, null, "Invalid credentials");

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
}

