using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public testController(InnoviaHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult TestConnection()
        {
            try
            {
                bool canConnect = _context.Database.CanConnect();
                return Ok(new { DatabaseConnected = canConnect });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fel vid anslutning: {ex.Message}");
            }
        }

        [HttpGet("timeslots")]
        public async Task<IActionResult> GetTimeSlots()
        {
            var TimeSlots = await _context.TimeSlots.ToListAsync();
            return Ok(TimeSlots);
        }

        [HttpGet("resources")]
        public async Task<IActionResult> GetResources()
        {
            var Resources = await _context.Resources.ToListAsync();
            return Ok(Resources);
        }

        [HttpGet("resourcetypes")]
        public async Task<IActionResult> GetResourceTypes()
        {
            var ResourceTypes = await _context.ResourceTypes.ToListAsync();
            return Ok(ResourceTypes);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDTO>> CreateBooking([FromBody] CreateBookingDTO bookingDto)
        {
            var booking = new Booking
            {
                Date = bookingDto.Date,
                ResourceId = bookingDto.ResourceId,
                TimeSlotId = bookingDto.TimeSlotId,
                UserId = "2" // Temporär hårdkodad användar-ID
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

            // Mappa till DTO, ersätt null med ""
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

            return CreatedAtAction(nameof(CreateBooking), new { id = result.BookingId }, result);
        }

        //Hämtar alla bokningar i databasen
        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
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

        //Endpoint för att ta bort en bokning ur databasen med dess id, vet inte om vi ska
        // använda denna, eller om vi ska sätta en "avbokad" istället för att admin ska kunna
        // se bokningar som blivit avbokade.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound(new { Message = $"Booking with id {id} not found" });
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Booking with id {id} was deleted" });
        }

    }
}
