using System;
namespace ResumeAnalyzerMVC.Responces
{
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
    }
}

