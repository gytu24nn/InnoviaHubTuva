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
            var resources = await _context.Resources
            .Select(r => new ResourceDTO
            {
                ResourceId = r.ResourcesId,
                ResourceTypeId = r.ResourceTypeId,
                Name = r.Name
            })
            .ToListAsync();
            return Ok(resources);
        }

        //AddResouerces
        [HttpPost("resources")]
        public async Task<IActionResult> AddResource([FromBody] CreateResourceDTO resourceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resource = new Resources
            {
                ResourceTypeId = resourceDto.ResourceTypeId,
                Name = resourceDto.Name
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllResources), new { id = resource.ResourcesId }, resource);
        }

        //ChangeResource
        [HttpPatch("resource/edit/{id}")]
        public async Task<IActionResult> ChangeResource()
        {
            return Ok();
        }

        //DeleteResource
        [HttpDelete("resource/{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound(new { Message = $"Resource with id {id} not found" });
            }

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            var deletedResource = new ResourceDTO
            {   
                ResourceId = resource.ResourcesId,
                ResourceTypeId = resource.ResourceTypeId,
                Name = resource.Name
            };
            return Ok(deletedResource);
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
