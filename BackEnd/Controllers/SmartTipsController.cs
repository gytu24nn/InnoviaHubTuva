using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BackEnd.Data;
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
        //[Authorize]
        public async Task<IActionResult> SmartTips([FromQuery] string? ResourceType, [FromQuery] DateTime? date, [FromQuery] int? timeSlotId, [FromQuery] int? resourceId)
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

            // samla ihop data i grupper.
            var BookingsGrouped = bookings
                .GroupBy(b => b.Date.Date)
                .Select(g => new { Date = g.Key, count = g.Count() })
                .OrderBy(g => g.count)
                .ToList();
            
            //Här sorteras bokningar efter datum och används sen i prompt för att skriva ut tips
            var leastBusyDayInWeek = BookingsGrouped.First().Date.ToString("dddd, dd MMMM");
            var mostBusyDayInWeek = BookingsGrouped.Last().Date.ToString("dddd, dd MMMM");

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
                    Håll tipset kort och använd gärna roliga och gulliga emojis. 
                ";
            }
            else
            {
                // här samlar vi ihop alla bokningar på en dag till en lista då den dagen användaren valt.
                var bookingsForday = bookings
                    .Where(b => b.Date.Date == date.Value.Date)
                    .ToList();

                // Hämtar alla resurser från databasen för att kunna se vilka resurser som är lediga för att kunna ge så passande tips som möjligt. 
                var allResources = await _context.Resources.ToListAsync();

                // Filtrerar ut de resurser som inte är bokade den valda dagen. 
                var freeResources = allResources
                    .Where(r => !bookingsForday.Any(b => b.ResourceId == r.ResourcesId))
                    .ToList();

                // Här är en if stats för att föreslå en resurs för användaren och om inga lediga resurser finns den dag skriva ut de. 
                string suggestedResource = freeResources.Any() ? freeResources.First().Name : "inga lediga resurser";

                // Först hämtas alla timeslots. 
                var allTimeSlots = await _context.TimeSlots.ToListAsync();

                // Här filtreras de timeslots som inte är bokade den valda dagen. 
                var bookedSlots = bookingsForday
                    .Where(b => b.ResourceId == resourceId)
                    .Select(b => b.TimeSlotId)
                    .ToList();

                // Väljer den första lediga timesloten av de som är lediga. 
                var availableSlot = allTimeSlots
                    .FirstOrDefault(ts => !bookedSlots.Contains(ts.TimeSlotsId));
                
                // För att ai ska förstå så skrivs den lediga timesloten om så den förstår eller meddelar att det inte finns några.
                string slotInfo = availableSlot != null ? $"{availableSlot.startTime:hh\\:mm}-{availableSlot.endTime:hh\\:mm}" : "inga lediga tider";

                // Här sammanfattas nu bokningssatus för dagen baserat på antalet bokningar.
                string bookingStatus = bookingsForday.Count == 0 ? "inga bokningar alls" : bookingsForday.Count < 3 ? "Några bokningar" : "Ganska fullt";

                // Skapar en lista med antal bokningar per datum (kan användas för statestik framåt)
                var allBookingSummary = bookings
                    .GroupBy(b => b.Date.Date)
                    .Select(g => $"{g.Key:yyyy-MM-dd}: {g.Count()} bokningar")
                    .ToList();

                // Här skrivs informationen in i prompten som sen används i content för att ai ska veta hur den ska skriva ut tipset.
                prompt = $@"
                Du är en rolig och trevlig assistent för InnoviaHubs bokningssystem 🎉

                Här är bokningsinformationen:
                - Resurstyp: {ResourceType ?? "okänd"}  
                - Datum: {date?.ToString("yyyy-MM-dd")}  
                - Lediga resurs att föreslå: {suggestedResource}
                - Status för vald dag: {bookingStatus}

                Ge användaren ett **kort och personligt tips** (1–2 meningar) för just denna resurstyp och dag.  
                Om det är många bokningar, föreslå ett lugnare alternativ.  
                Om det är få bokningar, uppmuntra användaren att boka!  
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
