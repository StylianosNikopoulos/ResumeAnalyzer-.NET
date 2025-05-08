using System;
using System.Collections.Generic;

namespace ResumeService.Models;

public partial class Keyword
{
    public int Id { get; set; }

    public string Keyword1 { get; set; } = null!;

    public virtual ICollection<ResumeKeyword> ResumeKeywords { get; set; } = new List<ResumeKeyword>();
}
