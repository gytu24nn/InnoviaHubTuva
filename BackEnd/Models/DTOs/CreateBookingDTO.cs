using System;

namespace BackEnd.Models.DTOs;

public class CreateBookingDTO
{
    public DateTime Date { get; set; }
    public int ResourceId { get; set; }
    public int TimeSlotId { get; set; }

}
