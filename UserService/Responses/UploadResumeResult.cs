using System;
namespace UserService.Responses
{
    public class UploadResumeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
    }
}

