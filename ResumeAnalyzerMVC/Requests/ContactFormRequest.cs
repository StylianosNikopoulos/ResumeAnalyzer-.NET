using System;
using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzerMVC.Requests
{
    public class ContactFormRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}

