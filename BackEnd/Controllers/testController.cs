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

    }
}
