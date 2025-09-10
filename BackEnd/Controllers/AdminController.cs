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
        [HttpDelete("resource/{id}")]
        public async Task<IActionResult> DeleteResource()
        {
            return Ok();
        }

        //addTimeslot
        [HttpPost("timeslots")]
        public async Task<IActionResult> AddTimeSlot()
        {
            return Ok();
        }

        //changeTimeslot
        [HttpPatch("timeslot/{id}")]
        public async Task<IActionResult> ChangeTimeSlot()
        {
            return Ok();
        }

        //deleteTimeslot
        [HttpDelete("timeslot/{id}")]
        public async Task<IActionResult> DeleteTimeSlot()
        {
            return Ok();
        }

    }
}
