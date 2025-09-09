using System;

namespace BackEnd.Models.DTOs;

public class ResourceDTO
{
    public int ResourceId { get; set; }
    public int ResourceTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ResourceTypeName { get; set; } = string.Empty;

}
