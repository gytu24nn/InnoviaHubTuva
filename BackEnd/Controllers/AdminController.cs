using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly InnoviaHubDbContext _context;

        public AdminController(InnoviaHubDbContext context)
        {
            _context = context;
        }

        // Hämtar alla bokningar
        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    Date = b.Date,
                    UserId = b.UserId ?? "",
                    UserEmail = b.User != null ? b.User.Email : "Okänd användare",
                    ResourceId = b.ResourceId,
                    ResourceName = b.Resource != null ? b.Resource.Name : "",
                    TimeSlotId = b.TimeSlotId,
                    StartTime = b.TimeSlot != null ? b.TimeSlot.startTime.ToString(@"hh\:mm") : "", // Formaterar tiden bättre visuellt
                    EndTime = b.TimeSlot != null ? b.TimeSlot.endTime.ToString(@"hh\:mm") : ""
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // Hämtar alla resurstyper
        [HttpGet("resourcetypes")]
        public async Task<IActionResult> GetAllResourceTypes()
        {
            var resourceTypes = await _context.ResourceTypes.ToListAsync();
            return Ok(resourceTypes);
        }

        // Hämtar alla resurser
        [HttpGet("resources")]
        public async Task<IActionResult> GetAllResources()
        {
            var resources = await _context.Resources
            .Include(r => r.ResourceType)
            .Select(r => new ResourceDTO
            {
                ResourceId = r.ResourcesId,
                ResourceTypeId = r.ResourceTypeId,
                Name = r.Name,
                ResourceTypeName = r.ResourceType != null ? r.ResourceType.Name : ""
            })
            .ToListAsync();
            return Ok(resources);
        }

        //Skapa en ny resurs
        [HttpPost("resources")]
        public async Task<IActionResult> AddResource([FromBody] CreateResourceDTO resourceDto)
        {

            // Kollar om alla [Required] fält är ifyllda, annars  => 400 Bad Request
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

        // Ändra en resurs, t.ex. namn eller typ. 
        [HttpPatch("resource/edit/{id}")]
        public async Task<IActionResult> EditResource( int id, [FromBody] EditResourceDTO editResourceDTO) 
        {
            var resource = await _context.Resources.FindAsync(id);

            if (resource == null)
            {
                return NotFound($" Finns ingen resurs med id: {id}");
            }

            resource.ResourceTypeId = editResourceDTO.ResourceTypeId;
            resource.Name = editResourceDTO.Name;

            await _context.SaveChangesAsync();

            var updatedResource = new ResourceDTO
            {
                ResourceId = resource.ResourcesId,
                ResourceTypeId = resource.ResourceTypeId,
                Name = resource.Name
            };

            return Ok(updatedResource);
        }

        // Ta bort en resurs
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
