using Microsoft.Extensions.Configuration;
using AtlasPatient.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using AtlasPatient.Models.Models;

namespace AtlasPatient.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetAuthTokenAsync()
        {
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

            return authResponse.jwt;
        }
    }
}
