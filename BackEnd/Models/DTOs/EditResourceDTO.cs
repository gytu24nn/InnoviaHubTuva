using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.DTOs;

public class EditResourceDTO
{
    [Required]
    public int ResourceTypeId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

}
