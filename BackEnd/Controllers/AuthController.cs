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
                    var roleClaim = (await _userManager.GetClaimsAsync(user))
                                        .FirstOrDefault(c => c.Type == "role")?.Value ?? "user";


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("SuperDuperUltraMegaSecureJWTSecretKeyThatIsLongEnough!");

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = "http://localhost:5199",
                        Audience = "http://localhost:5199",
                        Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("role", roleClaim),
                        new Claim("userId", user.Id),
                    }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    if (roleClaim.ToLower() == "user")
                    {
                        return Ok(new
                        {
                            user.Id,
                            user.UserName,
                            Role = roleClaim,
                            Token = tokenString,
                        });
                    }

                    // Om admin
                    return Ok(new { user.Id, user.UserName, Role = roleClaim, Token = tokenString });
                }
                return Unauthorized();
            }
        }
    }
}