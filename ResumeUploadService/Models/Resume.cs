using System;
using System.Collections.Generic;

namespace ResumeUploadService.Models;

public partial class Resume
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual ICollection<ResumeKeyword> ResumeKeywords { get; set; } = new List<ResumeKeyword>();
}
