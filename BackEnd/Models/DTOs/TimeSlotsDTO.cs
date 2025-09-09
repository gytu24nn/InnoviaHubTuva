namespace BackEnd.Models.DTOs
{
    // What you return to clients
    public class TimeSlotDTO
    {
        public int TimeSlotsId { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int Duration { get; set; }
    }

    // What clients send when creating/updating
    public class CreateTimeSlotDTO
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}