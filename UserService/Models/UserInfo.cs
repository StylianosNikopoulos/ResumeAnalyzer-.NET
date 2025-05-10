using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApplyService.Models;

public partial class UserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("resumePath")]
    public string ResumePath { get; set; } = null!;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
}
