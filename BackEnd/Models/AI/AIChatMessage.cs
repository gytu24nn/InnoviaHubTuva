using System;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BackEnd.Models.AI;

public class AIChatMessage
{
    public int id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsFromAI { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
