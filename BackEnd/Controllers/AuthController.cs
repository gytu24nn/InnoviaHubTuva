using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly IConfiguration _configuration;

        //Konstruktor
        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;

            _configuration = configuration;

        }

        // Skapa en ny användare (Används ej i prototypen)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var user = new AppUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (model.IsAdmin)
            {
                await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
            }

            var roleClaim = (await _userManager.GetClaimsAsync(user))
                                .FirstOrDefault(c => c.Type == "role")?.Value;
            return Ok(new { user.Id, user.UserName, Role = roleClaim });
        }

        // Logga in användare och skapa JWT-token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Hämta användarens roll
                    var roles = await _userManager.GetRolesAsync(user);
                    var roleClaim = roles.FirstOrDefault() ?? "user";

                    var Issuer = _configuration["JwtSettings:Issuer"] ?? "https://localhost:5099";
                    var Audience = _configuration["JwtSettings:Audience"] ?? "https://localhost:5099";
                    var Secret = _configuration["JwtSettings:Secret"] ?? "SuperDuperUltraMegaSecureJWTSecretKeyThatIsLongEnough!";
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, roleClaim),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                    };

                    var token = new JwtSecurityToken(
                        issuer: Issuer,
                        audience: Audience,
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: creds
                        );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    Response.Cookies.Append("auth", tokenString, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // sätt till true i produktion med HTTPS
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/"
                    });

                    return Ok(new { user.Id, user.UserName, Role = roleClaim });
                }
                return Unauthorized();
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                roles
            });

        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth", new CookieOptions { Path = "/" });
          
            return Ok();
        }
    }
}