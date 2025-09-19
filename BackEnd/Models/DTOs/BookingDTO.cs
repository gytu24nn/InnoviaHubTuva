namespace BackEnd.Models.DTOs;

public class BookingDTO
{
    public int BookingId { get; set; }
    public DateTime Date { get; set; }
    public string FormattedDate { get; set; } = "";
    public string UserId { get; set; } = "";
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = "";
    public int TimeSlotId { get; set; }
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string? UserName { get; set; } 
}
