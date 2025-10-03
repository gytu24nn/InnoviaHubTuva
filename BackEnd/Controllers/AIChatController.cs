using BackEnd.Data;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BackEnd.Models;


namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly InnoviaHubDbContext _context;
        private readonly IHubContext<BookingHub> _hubContext;

        // private static List<PendingBooking> _pendingBookings = new();

        public AIChatController(IHttpClientFactory httpClientFactory, InnoviaHubDbContext context, IHubContext<BookingHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> ChatAI()
        {
            return Ok();
        }

        
    }
}
