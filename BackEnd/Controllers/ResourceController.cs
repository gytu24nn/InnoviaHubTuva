using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResourceController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public ResourceController(InnoviaHubDbContext context)
        {
            _context = context;
        }

        //available time slots for a resource on a specific date
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

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableResources(
            [FromQuery] DateTime date,
            [FromQuery] int timeSlotId,
            [FromQuery] int? typeId = null)

        {
            var query = _context.Resources.Include(r => r.ResourceType).AsQueryable();

            if (typeId.HasValue)
            {
                query = query.Where(r => r.ResourceTypeId == typeId.Value);
            }

            var allResources = await query.ToListAsync();

            //Hämta resurser som redan är bokade
            var bookedIds = await _context.Bookings
                .Where(b => b.Date == date.Date && b.TimeSlotId == timeSlotId)
                .Select(b => b.ResourceId)
                .ToListAsync();

            //filtrera ut lediga resurser
            var availableResource = allResources
                .Where(r => !bookedIds.Contains(r.ResourcesId))
                .Select(r => new
                {
                    r.ResourcesId,
                    r.Name,
                    type = r.ResourceType.Name
                })
                .ToList();

            return Ok(availableResource);
        }

        [HttpGet("resourcetypes")]
        public async Task<IActionResult> GetResourceTypes()
        {
            var ResourceTypes = await _context.ResourceTypes.ToListAsync();
            return Ok(ResourceTypes);
        }

        [HttpGet("resourcetypes/{id}")]
        public async Task<IActionResult> GetResourcesType(int id)
        {
            var resourceType = await _context.ResourceTypes
            .Select(rt => new
            {
                rt.ResourceTypeId,
                rt.Name
            })
            .FirstOrDefaultAsync(rt => rt.ResourceTypeId == id);

            if (resourceType == null)
            {
                return NotFound();
            }

            return Ok(resourceType);
        }
    }
}
