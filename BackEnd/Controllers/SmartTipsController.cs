using System.Net.Http.Headers;
using BackEnd.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartTipsController : ControllerBase
    {
        private readonly IHttpClientFactory _IHttpClientFactory;
        private readonly InnoviaHubDbContext _context;

        public SmartTipsController(IHttpClientFactory IHttpClientFactory, InnoviaHubDbContext context)
        {
            _IHttpClientFactory = IHttpClientFactory;
            _context = context;
        }

        public async Task<IActionResult> SmartTips()
        {
            return Ok();
        }
    }
}
