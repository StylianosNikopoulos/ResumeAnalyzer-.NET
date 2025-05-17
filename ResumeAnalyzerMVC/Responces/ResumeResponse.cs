using System;
namespace ResumeAnalyzerMVC.Responces
{
    public class ResumeResponse<T>
    {
        public string Message { get; set; }
        public string Data { get; set; }
        public T Resumes { get; set; }
    }
}

