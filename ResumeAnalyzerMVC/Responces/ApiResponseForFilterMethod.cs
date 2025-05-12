using System;
namespace ResumeAnalyzerMVC.Responces
{
    public class ApiResponseForFilterMethod<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}

