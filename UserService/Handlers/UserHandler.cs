using System;
using ApplyService.Models;
using ApplyService.Responses;

namespace ApplyService.Handlers
{
	public class UserHandler
    {
        private readonly UserServiceDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;
        public UserHandler(UserServiceDbContext context, IHostEnvironment hostEnvironment)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		public async Task<UploadResumeResult> HandleAsync(IFormFile file, string name, string email)
		{
            if (file == null || file.Length == 0)
            {
                return new UploadResumeResult { Success = false, Message = "Invalid file" };
            }

            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Resumes", $"{Guid.NewGuid()}_{file.FileName}");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var newUserInfo = new UserInfo
            {
                Name = name,
                Email = email,
                ResumePath = filePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserInfos.Add(newUserInfo);
            await _context.SaveChangesAsync();

            return new UploadResumeResult
            {
                Success = true,
                Message = "Resume uploaded successfully",
                UserId = newUserInfo.Id
            };
        }
    }
}

