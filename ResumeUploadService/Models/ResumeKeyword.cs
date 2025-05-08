using System;
using System.Collections.Generic;

namespace ResumeService.Models;

public partial class ResumeKeyword
{
    public int Id { get; set; }

    public int ResumeId { get; set; }

    public int KeywordId { get; set; }

    public virtual Keyword Keyword { get; set; } = null!;

    public virtual Resume Resume { get; set; } = null!;
}
