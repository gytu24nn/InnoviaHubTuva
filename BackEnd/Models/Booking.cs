using System;

namespace BackEnd.Models;

public class Booking
{
    public int BookingId { get; set; }

    public int ResourceId { get; set; }
    public Resources Resource { get; set; }

    public int TimeSlotId { get; set; }
    public TimeSlots TimeSlot { get; set; }

    public DateTime Date { get; set; }       // vilken dag bokningen gäller
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public string UserName { get; set; }
}
