using System;
using UserService.Models;

namespace ResumesService.Responces
{
	public class ResumesResult
	{
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<UserInfo> UserInfos { get; set; }
    }
}

