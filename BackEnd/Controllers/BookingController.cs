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

            var selectedSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotsId == booking.TimeSlotId);

            if (selectedSlot == null) return BadRequest("Invalid timeslot.");

            // Kontrollera överlappande bokningar
            var overlap = await _context.Bookings
                .Include(b => b.TimeSlot)
                .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                            b.Date.Date == booking.Date.Date &&
                            b.TimeSlot != null && (
                                (selectedSlot.startTime < b.TimeSlot.endTime) &&
                                (selectedSlot.endTime > b.TimeSlot.startTime)
                            )
                        );

            if (overlap) return BadRequest("Time slot already booked");

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Hämta bokningen med relaterade data
            var createdBooking = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            if (createdBooking == null) return NotFound();

            // Mappa till DTO
            var result = new BookingDTO
            {
                BookingId = createdBooking.BookingId,
                Date = createdBooking.Date,
                UserId = createdBooking.UserId ?? "",
                UserName = createdBooking.User?.UserName ?? "",
                ResourceId = createdBooking.ResourceId,
                ResourceName = createdBooking.Resource?.Name ?? "",
                TimeSlotId = createdBooking.TimeSlotId,
                StartTime = createdBooking.TimeSlot?.startTime.ToString(@"hh\:mm") ?? "",
                EndTime = createdBooking.TimeSlot?.endTime.ToString(@"hh\:mm") ?? ""
            };

            await _hubContext.Clients.All.SendAsync("BookingCreated", result);

            return CreatedAtAction(nameof(CreateBooking), new { id = result.BookingId }, result);
        }

        // Get a single booking by ID (for BookingConfirmed page)
        [HttpGet("{id}")]
        // [Authorize] // uncomment later if you want to restrict
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound(new { Message = $"Booking with id {id} not found" });

            var result = new BookingDTO
            {
                BookingId = booking.BookingId,
                Date = booking.Date,
                UserId = booking.UserId ?? "",
                UserName = booking.User?.UserName ?? "",
                ResourceId = booking.ResourceId,
                ResourceName = booking.Resource?.Name ?? "",
                TimeSlotId = booking.TimeSlotId,
                StartTime = booking.TimeSlot?.startTime.ToString(@"hh\:mm") ?? "",
                EndTime = booking.TimeSlot?.endTime.ToString(@"hh\:mm") ?? ""
            };

            return Ok(result);
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
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized("UserId not found in token.");

    // Hämta bokningar där UserId matchar, hantera null och trim
    var bookings = await _context.Bookings
        .Where(b => b.UserId != null && b.UserId.Trim() == userId)
        .Include(b => b.Resource)
        .Include(b => b.TimeSlot)
        .Include(b => b.User)
        .Select(b => new BookingDTO
        {
            BookingId = b.BookingId,
            Date = b.Date,
            UserId = b.UserId ?? "",
            UserName = b.User != null ? b.User.UserName : "",
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
