using System;
namespace ResumeAnalyzerMVC.Requests
{
    public class UploadResumeRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IFormFile File { get; set; }
    }
}

