using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ResumePath { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
