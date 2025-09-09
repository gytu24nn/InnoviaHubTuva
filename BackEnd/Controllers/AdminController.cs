using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public AdminController(InnoviaHubDbContext context)
        {
            _context = context;
        }

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

        [HttpGet("resources")]
        public async Task<IActionResult> GetAllResources()
        {
            var Resources = await _context.Resources.ToListAsync();
            return Ok(Resources);
        }

        //AddResouerces
        [HttpPost("resources")]
        public async Task<IActionResult> AddResource()
        {
            return Ok();
        }

        //ChangeResource
        [HttpPatch("resource/{id}")]
        public async Task<IActionResult> ChangeResource()
        {
            return Ok();
        }

        //DeleteResource
        [HttpPatch("resource/{id}")]
        public async Task<IActionResult> DeleteResource()
        {
            return Ok();
        }

        // Hämtar alla tidsluckor
        [HttpGet("timeslots")]
        public async Task<IActionResult> GetAllTimeSlots()
        {
            var slots = await _context.TimeSlots
                .Select(t => new TimeSlotDTO
                {
                    TimeSlotsId = t.TimeSlotsId,
                    StartTime = t.startTime.ToString(@"hh\\:mm"),
                    EndTime = t.endTime.ToString(@"hh\\:mm"),
                    Duration = t.Duration
                })
                .ToListAsync();

            return Ok(slots);
        }

        // Skapa en ny tidslucka
        [HttpPost("timeslots")]
        public async Task<IActionResult> AddTimeSlot([FromBody] CreateTimeSlotDTO dto)
        {
            if (!ModelState.IsValid || dto.StartTime >= dto.EndTime)
            {
                return BadRequest("Ogiltig tidslucka.");
            }

            // Kontrollera överlappning
            bool overlapExists = await _context.TimeSlots.AnyAsync(t =>
                (dto.StartTime < t.endTime && dto.EndTime > t.startTime)
            );

            if (overlapExists)
            {
                return Conflict("Det finns redan en tidslucka som överlappar den angivna tiden.");
            }

            var duration = (int)(dto.EndTime - dto.StartTime).TotalMinutes;

            var newTimeSlot = new TimeSlots
            {
                startTime = dto.StartTime,
                endTime = dto.EndTime,
                Duration = duration
            };

            _context.TimeSlots.Add(newTimeSlot);
            await _context.SaveChangesAsync();

            var result = new TimeSlotDTO
            {
                TimeSlotsId = newTimeSlot.TimeSlotsId,
                StartTime = newTimeSlot.startTime.ToString(@"hh\\:mm"),
                EndTime = newTimeSlot.endTime.ToString(@"hh\\:mm"),
                Duration = newTimeSlot.Duration
            };

            return CreatedAtAction(nameof(GetAllTimeSlots), new { id = newTimeSlot.TimeSlotsId }, result);
        }

        // Ändra en tidslucka
        [HttpPatch("timeslot/{id}")]
        public async Task<IActionResult> ChangeTimeSlot(int id, [FromBody] CreateTimeSlotDTO dto)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound($"Ingen tidslucka hittades med id {id}");
            }

            if (dto.StartTime >= dto.EndTime)
            {
                return BadRequest("Ogiltig tidslucka.");
            }

            // Kontrollera överlappning, men exkludera den nuvarande tidsluckan
            bool overlapExists = await _context.TimeSlots.AnyAsync(t =>
                t.TimeSlotsId != id &&
                (dto.StartTime < t.endTime && dto.EndTime > t.startTime)
            );

            if (overlapExists)
            {
                return Conflict("Det finns redan en annan tidslucka som överlappar den angivna tiden.");
            }

            timeSlot.startTime = dto.StartTime;
            timeSlot.endTime = dto.EndTime;
            timeSlot.Duration = (int)(dto.EndTime - dto.StartTime).TotalMinutes;

            await _context.SaveChangesAsync();

            var updatedTimeSlot = new TimeSlotDTO
            {
                TimeSlotsId = timeSlot.TimeSlotsId,
                StartTime = timeSlot.startTime.ToString(@"hh\\:mm"),
                EndTime = timeSlot.endTime.ToString(@"hh\\:mm"),
                Duration = timeSlot.Duration
            };

            return Ok(updatedTimeSlot);
        }

        // Ta bort en tidslucka
        [HttpDelete("timeslot/{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound(new { Message = $"Tidslucka med id {id} hittades inte" });
            }

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            var deletedTimeSlot = new TimeSlotDTO
            {
                TimeSlotsId = timeSlot.TimeSlotsId,
                StartTime = timeSlot.startTime.ToString(@"hh\\:mm"),
                EndTime = timeSlot.endTime.ToString(@"hh\\:mm"),
                Duration = timeSlot.Duration
            };

            return Ok(deletedTimeSlot);
        }

    }
}
