﻿using System;
using System.Collections.Generic;

namespace AuthService.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int? RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Role? Role { get; set; }
}
