using BackEnd.Data;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Alla inloggade användare kan nå dessa endpoints
    public class UserController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public UserController(InnoviaHubDbContext context)
        {
            _context = context;
        }


        [HttpGet("timeslots")]
        public async Task<IActionResult> GetAllTimeSlots()
        {
            try
            {
                var slots = await _context.TimeSlots
                    .Select(t => new TimeSlotDTO
                    {
                        TimeSlotsId = t.TimeSlotsId,
                        StartTime = t.startTime.ToString(@"hh\:mm"),
                        EndTime = t.endTime.ToString(@"hh\:mm"),
                        Duration = t.Duration
                    })
                    .ToListAsync();

                return Ok(slots);
            }
            catch (Exception ex)
            {
                // Log the exact error to backend console
                Console.WriteLine($"❌ Error in GetAllTimeSlots: {ex.Message}");
                return StatusCode(500, "Something went wrong when fetching timeslots.");
            }
        }

        // Hämtar bokningar för användare (utan känslig data)
        [HttpGet("public-bookings")]
        public async Task<IActionResult> GetPublicBookings()
        {
            var bookings = await _context.Bookings
                .Select(b => new
                {
                    BookingId = b.BookingId,
                    Date = b.Date,
                    TimeSlotId = b.TimeSlotId,
                    ResourceId = b.ResourceId
                })
                .ToListAsync();

            return Ok(bookings);
        }

    }
}
