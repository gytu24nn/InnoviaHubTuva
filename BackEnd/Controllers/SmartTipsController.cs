using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BackEnd.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> SmartTips()
        {
            return Ok();
        }
    }
}
