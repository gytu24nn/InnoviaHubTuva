using BackEnd.Data;
using BackEnd.Models;
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

        // [HttpPost("addbookings")]
        // public async Task<IActionResult> AddBooking([FromBody] Booking newBooking)
        // {
        //     if (newBooking == null)
        //     {
        //         return BadRequest("Bokningsdata saknas.");
        //     }

        //     _context.Bookings.Add(newBooking);
        //     await _context.SaveChangesAsync();

        //     return Ok(new { Message = "Bokning tillagd!", BookingId = newBooking.Id });
        // }

    }
}
