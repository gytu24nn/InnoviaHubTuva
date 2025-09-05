using System;
using Microsoft.AspNetCore.SignalR;

namespace BackEnd.Hubs;

public class BookingHub : Hub
{
    // public async Task BookingCreated(int resourceId, DateTime date, int timeSlotId)
    // {
    //     await Clients.Others.SendAsync("BookingCreated", resourceId, date, timeSlotId);
    // }

    // public async Task BookingCancelled(int resourceId, DateTime date, int timeSlotId)
    // {
    //     await Clients.Others.SendAsync("BookingCancelled", resourceId, date, timeSlotId);
    // }
}
