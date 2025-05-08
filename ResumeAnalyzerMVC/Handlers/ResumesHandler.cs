using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthService.Enum;
using AuthService.AuthRequest;

namespace ResumeAnalyzerMVC.Handlers
{
    public class ResumesHandler
    {

        private readonly HttpClient _httpClient;


        public ResumesHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
        }
    }
}