using System;

namespace BackEnd.Models.AI;

// Denna innehåller "väntande" bokning som AI föreslår och användaren kan bekräfta. 
public class PendingBooking
{
    public string Date { get; set; } = string.Empty;
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int TimeSlotId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}
