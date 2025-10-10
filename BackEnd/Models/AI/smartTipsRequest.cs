using System;

namespace BackEnd.Models.AI;

public class smartTipsRequest
{
    public string? ResourceType { get; set; }
    public DateTime? Date { get; set; }
    public int? ResourceId { get; set; }
}
