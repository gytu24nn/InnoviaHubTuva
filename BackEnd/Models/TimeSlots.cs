using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BackEnd.Models;

public class TimeSlots
{
    public int Id { get; set; }
    public TimeSpan startTime { get; set; }
    public TimeSpan endTime { get; set; }
    public int Duration { get; set; }

    public ICollection<Booking> Bookings { get; set; }
}
