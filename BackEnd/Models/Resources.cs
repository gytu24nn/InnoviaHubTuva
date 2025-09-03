using System;

namespace BackEnd.Models;

public class Resources
{
    public int Id { get; set; }              // Ex: 1..15, 101..104, 201..204, 301
    public int ResourceTypeId { get; set; }  // FK till ResourceType
    public required string Name { get; set; }         // Ex: "Skrivbord 1", "Mötesrum 2", ...

    public ResourceType ResourceType { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}
