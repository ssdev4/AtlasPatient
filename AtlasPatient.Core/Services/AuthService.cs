using Microsoft.Extensions.Configuration;
using AtlasPatient.Core.IServices;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AtlasPatient.Models.Models;
using Microsoft.Extensions.Logging;

namespace AtlasPatient.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private string _cachedToken;
        private DateTime _tokenExpiration;

        public AuthService(HttpClient httpClient, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow)
            {
                _logger.LogInformation("Using cached token");
                return _cachedToken;
            }

            _logger.LogInformation("Requesting new token");
            var authUrl = "https://testapi.mindware.us/auth/local";
            var authData = new
            {
                identifier = _configuration["Auth:Identifier"],
                password = _configuration["Auth:Password"]
            };

            var content = new StringContent(JsonSerializer.Serialize(authData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(authUrl, content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

            _cachedToken = authResponse.jwt;
            _tokenExpiration = DateTime.UtcNow.AddMinutes(25); // Assuming the token is valid for 30 minutes

            return _cachedToken;
        }
    }
}
