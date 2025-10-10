using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
        [Authorize]
        public async Task<IActionResult> SmartTips([FromQuery] string? ResourceType, [FromQuery] DateTime? date, [FromQuery] int? resourceId)
        {
            // Kolla om user id finns i token.
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier)?.Trim();
            if (string.IsNullOrEmpty(userid))
                return Unauthorized("Userid not found in token");

            //Hämta all information om bookings.
            var bookings = await _context.Bookings.ToListAsync();

            // Kolla om det finns några bokningar.
            if (bookings.Count == 0)
            {
                return Ok(new { tip = "Det finns inga bokningar ännu - passa på att boka när som helst" });
            }

            var today = DateTime.Today;

            // samla ihop data i grupper.
            var BookingsGrouped = bookings
                .Where(b => b.Date.ToLocalTime().Date >= today) // Endast idag eller framåt
                .GroupBy(b => b.Date.ToLocalTime().Date)
                .Select(g => new { Date = g.Key, count = g.Count() })
                .ToList();

            if (BookingsGrouped.Count == 0)
            {
                // Detta hanteras redan ovan men kan vara en bra "fallback"
                return Ok(new { tip = "Inga framtida bokningar ännu, passa på att boka när det passar dig! 🌟" });
            }

            // Hitta dagen med minst bokningar bland de framtida
            var leastBusyDay = BookingsGrouped
                .OrderBy(g => g.count)      // Sortera efter lägst antal bokningar
                .ThenBy(g => g.Date)        // Välj det tidigaste datumet om det är lika antal
                .First();

            // Hitta dagen med mest bokningar bland de framtida
            var mostBusyDay = BookingsGrouped
                .OrderByDescending(g => g.count) // Sortera efter högst antal bokningar
                .ThenBy(g => g.Date)             // Välj det tidigaste datumet om det är lika antal
                .First();

            var swedishCulture = new CultureInfo("sv-SE");

            var leastBusyDayInWeek = leastBusyDay.Date
                .ToString("dddd dd MMMM", swedishCulture);

            var mostBusyDayInWeek = mostBusyDay.Date
                .ToString("dddd dd MMMM", swedishCulture);

            string prompt;

            // Här kontrollerar vi om vi har ett datum/vald dag om det inte finns en vald dag ska ha kolla bokningar och skriva ut ett tips som passar användaren och den resurs den valt. 
            if (!date.HasValue)
            {
                prompt = $@"
                    Du är en rolig och trevlig assistent för InnoviaHubs bokningssystem.
                    
                    Resurstyp: {ResourceType ?? "Valfri resurs"}

                    - Mest bokade i veckan: {mostBusyDayInWeek} 
                    - Minst bokade i veckan: {leastBusyDayInWeek}

                    Baserat på bokningsdata, ge ett kort personligt tips till användaren om vilken dag som kan vara bra att boka.
                    Håll tipset kort och använd gärna roliga och gulliga emojis. Ge tips endast på framtida dagar ignorera dagar som redan passerat.
                ";
            }
            else
            {
                var targetDate = date.Value.Date;
                // här samlar vi ihop alla bokningar på en dag till en lista då den dagen användaren valt.
                var bookingsForday = bookings
                    .Where(b => b.Date.Date == targetDate)
                    .ToList();

                // Istället för suggestedResource
                var selectedResource = await _context.Resources.FirstOrDefaultAsync(r => r.ResourcesId == resourceId);
                string resourceName = selectedResource?.Name ?? "resurs";

                // Först hämtas alla timeslots. 
                var allTimeSlots = await _context.TimeSlots.ToListAsync();

                // Här sammanfattas nu bokningssatus för dagen baserat på antalet bokningar.
                string bookingStatus = bookingsForday.Count == 0 ? "inga bokningar alls" : bookingsForday.Count < 3 ? "Några bokningar" : "Ganska fullt";

                // Här skrivs informationen in i prompten som sen används i content för att ai ska veta hur den ska skriva ut tipset.
                prompt = $@"
                Du är en trevlig assistent för InnoviaHubs bokningssystem 🎉

                Här är bokningsinformationen:
                - Datum: {date:yyyy-MM-dd} (använd exakt detta datum och skriv det inte om)
                - Resurs: {resourceName} (använd exakt detta namn)
                - Bokningsstatus: {bookingStatus}

                Ge användaren ett **kort och personligt tips** (1–2 meningar).  
                - Om det är många bokningar, föreslå ett lugnare alternativ.  
                - Om det är få bokningar, uppmuntra användaren att boka!  
                - Skriv endast om datumet ovan.  
                Använd emojis och en glad ton, men håll det kort och relevant.  
                ";
            }

            // Här skapas ai och man skriver vilken gpt model man vill använda samt skapar rollerna. 
            var http = _IHttpClientFactory.CreateClient("openai");
            var body = new
            {
                model = "gpt-4.1",

                input = new object[]
                {
                    new {
                        role = "system",
                        content = prompt
                    },
                    new {
                        role = "user",
                        content = prompt
                    }
                },
            };

            //Skapar innehållet som ska skickas till OpenAI API:t. Gör också en seralisering.
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            // Skickar informationen till open ai till deras endpoint responses med json innehåll bland annat prompt.
            var response = await http.PostAsync("responses", content);
            // Läser av svaret som text från API:et. 
            var raw = await response.Content.ReadAsStringAsync();

            System.Console.WriteLine("raw" + raw);

            try
            {
                //Försöker tolka (parsa) det råa svaret som JSON så att vi kan läsa strukturerad data ur det.
                var doc = JsonDocument.Parse(raw);
                //Hämtar självs textsvaret från openai och sen navigerar vi igenom Json i det olika stegen tills det blir text. 
                var answer = doc.RootElement
                    .GetProperty("output")[0]
                    .GetProperty("content")[0]
                    .GetProperty("text").GetString();
                
                // här returnerar vi tipset som Json till klienten. 
                return Ok(new { tip = answer });
            } catch {
                // Om något då går fel vid hämtningar eller omskriv så returneras ett standard fel. 
                return Ok(new { tip = "Kunde inte hämta tips just nu prova igen senare 🙈" });
            }
        }
    }
}
