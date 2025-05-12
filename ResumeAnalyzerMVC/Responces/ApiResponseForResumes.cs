using System;
namespace ResumeAnalyzerMVC.Responces
{
    public class ApiResponseForResumes<T>
    {
        public string Message { get; set; }
        public T Resumes { get; set; }
    }
}

