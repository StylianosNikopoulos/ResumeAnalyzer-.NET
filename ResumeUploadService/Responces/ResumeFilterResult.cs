using System;
using ApplyService.Models;

namespace ResumesService.Responces
{
	public class ResumeFilterResult
	{
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<UserInfo> UserInfos { get; set; }
    }
}

