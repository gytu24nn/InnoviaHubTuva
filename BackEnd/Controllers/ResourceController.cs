using BackEnd.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public ResourceController(InnoviaHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("{resourceId}/available-timeslots")]
        public async Task<IActionResult> GetAvailableTimeSlots(int resourceId, [FromQuery] DateTime date)
        {
            var allSlots = await _context.TimeSlots.ToListAsync();

            var bookedSlotsIds = await _context.Bookings
                .Where(b => b.ResourceId == resourceId && b.Date.Date == date.Date)
                .Select(b => b.TimeSlotId)
                .ToListAsync();

            var availableSlots = allSlots
                .Where(slot => !bookedSlotsIds.Contains(slot.TimeSlotsId))
                .ToList();
            
            return Ok(availableSlots);
        }

    }
}
