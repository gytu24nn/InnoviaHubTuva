using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.DTOs;
using BackEnd.Models; 
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BackEnd.Hubs;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        private readonly InnoviaHubDbContext _context;
        private readonly IHubContext<BookingHub> _hubContext;

        public BookingController(InnoviaHubDbContext context, IHubContext<BookingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] CreateBookingDTO bookingDto)
        {
            // Hämta inloggad användares Id från token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.Trim();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            var booking = new Booking
            {
                Date = bookingDto.Date,
                ResourceId = bookingDto.ResourceId,
                TimeSlotId = bookingDto.TimeSlotId,
                UserId = userId   
            };

            // Kontrollera överlappande bokningar
            var overlap = await _context.Bookings
                .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                            b.Date.Date == booking.Date.Date &&
                            b.TimeSlotId == booking.TimeSlotId);

            if (overlap) return BadRequest("Time slot already booked");

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Hämta bokningen med relaterade data
            var createdBooking = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            if (createdBooking == null) return NotFound();

            // Mappa till DTO
            var result = new BookingDTO
            {
                BookingId = createdBooking.BookingId,
                Date = createdBooking.Date,
                UserId = createdBooking.UserId ?? "",
                ResourceId = createdBooking.ResourceId,
                ResourceName = createdBooking.Resource?.Name ?? "",
                TimeSlotId = createdBooking.TimeSlotId,
                StartTime = createdBooking.TimeSlot?.startTime.ToString(@"hh\:mm") ?? "",
                EndTime = createdBooking.TimeSlot?.endTime.ToString(@"hh\:mm") ?? ""
            };

            await _hubContext.Clients.All.SendAsync("BookingCreated", result);

            return CreatedAtAction(nameof(CreateBooking), new { id = result.BookingId }, result);
        }

        //Endpoint för att ta bort en bokning ur databasen med dess id, vet inte om vi ska
        // använda denna, eller om vi ska sätta en "avbokad" istället för att admin ska kunna
        // se bokningar som blivit avbokade.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound(new { Message = $"Booking with id {id} not found" });
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            var result = new BookingDTO
            {
                BookingId = booking.BookingId,
                Date = booking.Date,
                UserId = booking.UserId ?? "",
                ResourceId = booking.ResourceId,
                ResourceName = booking.Resource?.Name ?? "",
                TimeSlotId = booking.TimeSlotId,
                StartTime = booking.TimeSlot?.startTime.ToString(@"hh\:mm") ?? "",
                EndTime = booking.TimeSlot?.endTime.ToString(@"hh\:mm") ?? ""
            };

            await _hubContext.Clients.All.SendAsync("BookingCancelled", result);

            return Ok(new { Message = $"Booking with id {id} was deleted" });
        }
        
[HttpGet("mybookings")]
[Authorize]
public async Task<IActionResult> GetAllMyBookings()
{
    // Hämta inloggad användares Id från token
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.Trim();

    if (string.IsNullOrEmpty(userId))
        return Unauthorized("UserId not found in token.");

    // Hämta bokningar där UserId matchar, hantera null och trim
    var bookings = await _context.Bookings
        .Where(b => b.UserId != null && b.UserId.Trim() == userId)
        .Include(b => b.Resource)
        .Include(b => b.TimeSlot)
        .Select(b => new BookingDTO
        {
            BookingId = b.BookingId,
            Date = b.Date,
            UserId = b.UserId ?? "",
            ResourceId = b.ResourceId,
            ResourceName = b.Resource != null ? b.Resource.Name : "",
            TimeSlotId = b.TimeSlotId,
            StartTime = b.TimeSlot != null ? b.TimeSlot.startTime.ToString(@"hh\:mm") : "",
            EndTime = b.TimeSlot != null ? b.TimeSlot.endTime.ToString(@"hh\:mm") : ""
        })
        .ToListAsync();

    return Ok(bookings);
}

    }
}
