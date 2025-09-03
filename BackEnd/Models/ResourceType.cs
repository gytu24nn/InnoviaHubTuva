using System;

namespace BackEnd.Models;

public class ResourceType
{
    public int Id { get; set; }          // 1=Desk, 2=Room, 3=VR, 4=AI
    public required string Name { get; set; }     // "Drop-in skrivbord", "Mötesrum", ...
    public ICollection<Resources> Resources { get; set; }
}
