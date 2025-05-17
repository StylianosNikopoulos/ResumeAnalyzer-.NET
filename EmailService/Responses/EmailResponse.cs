using System;
namespace EmailService.Responses
{
    public class EmailResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}

