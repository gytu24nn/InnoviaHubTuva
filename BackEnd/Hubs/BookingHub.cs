using System;
using BackEnd.Data;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Hubs;

public class BookingHub : Hub
{
    private readonly InnoviaHubDbContext _context;

    public BookingHub(InnoviaHubDbContext context)
    {
        _context = context;
    }

    public async Task SendBookingsForResource(int resourceId, DateTime date)
    {
        var bookings = await _context.Bookings
            .Include(b => b.TimeSlot)
            .Include(b => b.Resource)
            .Where(b => resourceId == resourceId && b.Date.Date == date.Date)
            .Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                Date = b.Date,
                UserId = b.UserId ?? "",
                ResourceId = b.ResourceId,
                ResourceName = b.Resource.Name ?? "",
                TimeSlotId = b.TimeSlotId,
                StartTime = b.TimeSlot != null ? b.TimeSlot.startTime.ToString(@"hh\:mm") : "",
                EndTime = b.TimeSlot != null ? b.TimeSlot.endTime.ToString(@"hh\:mm") : ""
            })
            .ToListAsync();

        await Clients.Caller.SendAsync("ReceiveBookingsForResource", bookings);
    }


}
