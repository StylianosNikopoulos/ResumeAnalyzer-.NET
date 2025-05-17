using System;
namespace ResumeAnalyzerMVC.Responses
{
    public class ResumeResponse<T>
    {
        public string Message { get; set; }
        public string Data { get; set; }
        public T Resumes { get; set; }
    }
}

