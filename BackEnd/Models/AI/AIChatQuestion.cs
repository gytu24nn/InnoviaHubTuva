using System;

namespace BackEnd.Models.AI;

public class AIChatQuestion
{
    // Här är klassen där används när användaren skickar en fråga.
    // UserId ut kommenterat för de kontrolleras i controller. 
    public string Question { get; set; } = string.Empty;
    // public int UserId { get; set; }
}
