using BackEnd.Data;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BackEnd.Models.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sprache;
using System.Text.Json;
using System.Text;
using BackEnd.Migrations;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.VisualBasic;


namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly InnoviaHubDbContext _context;
        private readonly IHubContext<BookingHub> _hubContext;
        private static readonly Dictionary<string, PendingBooking> _pendingBookings = new();

        // private static List<PendingBooking> _pendingBookings = new();

        public AIChatController(IHttpClientFactory httpClientFactory, InnoviaHubDbContext context, IHubContext<BookingHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("chat")]
        //[Authorize]
        public async Task<IActionResult> ChatAI([FromBody] AIChatQuestion aIChatQuestion)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.Trim();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token.");

            // Spara användarens fråga för att sen skicka med den som kontext.
            var userMsg = new AIChatMessage { UserId = userId, Message = aIChatQuestion.Question, IsFromAI = false };
            _context.AIChatMessages.Add(userMsg);
            await _context.SaveChangesAsync();

            // Hämtar de senaste 10 meddelandena för kontext.
            var lastMessages = await _context.AIChatMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            var conversation = lastMessages.Select(m => new
            {
                role = m.IsFromAI ? "assistant" : "user",
                content = m.Message
            }).ToList();

            var todayDate = DateTime.Today.ToString("yyyy-MM-dd");

            // Systemprompt som styr AI:s beteende och den kopplas sen i new role
            var systemPrompt = $@"
                Du är en AI-assistent för InnoviaHubs bokningssystem. 

                Idag är det {todayDate}

                Du kan: 
                - svara på frågor om bokningar, resurser, tider och systemet.
                - Tolka om användaren vill boka en resurs (ex. 'Jag vill boka mötesrum på tisdag 8h').
                - Tolka naturliga datumuttryck som 'imorgon', 'idag', 'på fredag' till YYYY-MM-DD (t.ex. 'imorgon' = dagens datum + 1 ).
                - TOlka tider som 'kl 10' till StartTime = '10:00 och välja en passande timeSlot.
                - Returnera alltid JSON: 
                {{
                    'AIResponse': 'Text som AI säger till användaren',
                    'IsBookingSuggestion': true/false,
                    'SuggestedBooking': {{
                        'Date': 'YYYY-MM-DD',
                        'ResourceId': 0,
                        'ResourceName': '',
                        'ResourceType': '',
                        'TimeSlotId': 0,
                        'StartTime': 'HH:mm',
                        'EndTime': 'HH:mm
                    }}
                }}

                Om användaren bara ställer en fråga, returnera JSON med 'IsBookingSuggestion': false och svara i 'AIResponse'.
                Om AI inte får tillräcklig info, lämna fält tomma så backend kan fylla i.
                Om AI föreslår en bokning, be användaren bekräfta innan den genomförs.
                Om AI inte kan tolka datum eller tid, lämna fält tomma så backend kan fylla i.

                Exempel:
                Om användaren skriver 'Jag vill boka ett mötesrum imorgon kl 10', sätt 'ResourceType' till 'Mötesrum'.";

            var http = _httpClientFactory.CreateClient("openai");


            var body = new
            {
                model = "gpt-4.1",

                input = new object[]
                {
                    new {role = "system", content = systemPrompt}
                }.Concat(conversation).ToArray()
            };

            // Skickar request till AI
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await http.PostAsync("responses", content);
            var raw = await response.Content.ReadAsStringAsync();

            System.Console.WriteLine("raw:" + raw);

            // Parsar AI:s svar (tolkar och bryter ner data från ett format till ett annat)
            string jsonResponse = "inget svar";
            try
            {
                var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("output", out var outputElem) &&
                    outputElem.GetArrayLength() > 0 &&
                    outputElem[0].TryGetProperty("content", out var contentElem) &&
                    contentElem.GetArrayLength() > 0 &&
                    contentElem[0].TryGetProperty("text", out var textElem))
                {
                    jsonResponse = textElem.GetString() ?? "inget svar";
                }
                else
                {
                    System.Console.WriteLine("JSON-format oväntat: " + raw);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("JSON-fel: " + ex.Message);
            }
            // kollar om svaret från AI är tomt om det är de returneras ett felmeddelande.
            if (string.IsNullOrEmpty(jsonResponse))
                return BadRequest("AI did not return a valid response");

            // Här omvandlar vi data från t.ex. json-form till C#
            AIChatResponse aIChatResponse;
            try
            {
                aIChatResponse = JsonSerializer.Deserialize<AIChatResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AIChatResponse();
            }
            catch
            {
                aIChatResponse = new AIChatResponse { AIResponse = jsonResponse ?? "Okänt svar.", IsBookingSuggestion = false };
            }

            if (aIChatResponse.IsBookingSuggestion != null)
            {
                var booking = aIChatResponse.SuggestedBooking;

                if (!string.IsNullOrEmpty(booking.Date) && booking.Date.ToLower() == "idag")
                {
                    booking.Date = DateTime.Today.ToString("yyyy-MM-dd");
                }
                else if (!string.IsNullOrEmpty(booking.Date) && booking.Date.ToLower() == "imorgon")
                {
                    booking.Date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                }
                else if (!string.IsNullOrEmpty(booking.Date) && booking.Date.ToLower().StartsWith("på "))
                {
                    var dayName = booking.Date.ToLower().Replace("på ", "").Trim();
                    var daysOfWeek = new[] { "söndag", "måndag", "tisdag", "onsdag", "torsdag", "fredag", "lördag" };
                    var today = DateTime.Today;
                    int todayIndex = (int)today.DayOfWeek;
                    int targetIndex = Array.IndexOf(daysOfWeek, dayName);
                    if (targetIndex >= 0)
                    {
                        int daysUntil = (targetIndex - todayIndex + 7) % 7;
                        daysUntil = daysUntil == 0 ? 7 : daysUntil;
                        booking.Date = today.AddDays(daysUntil).ToString("yyyy-MM-dd");
                    }
                }

                if (booking.ResourceId == 0 && !string.IsNullOrEmpty(booking.ResourceName))
                {
                    var resource = await _context.Resources
                        .FirstOrDefaultAsync(r => r.Name.ToLower().Contains(booking.ResourceName.ToLower()));
                    if (resource != null)
                    {
                        booking.ResourceId = resource.ResourcesId;
                        booking.ResourceName = resource.Name;
                    }
                }

                if (booking.ResourceId == 0 && !string.IsNullOrEmpty(booking.ResourceType))
                {
                    var resource = await _context.Resources
                        .Include(r => r.ResourceType)
                        .Where(r => r.ResourceType.Name.ToLower().Contains(booking.ResourceType.ToLower()))
                        .OrderBy(r => Guid.NewGuid())
                        .FirstOrDefaultAsync();

                    if (resource != null)
                    {
                        booking.ResourceId = resource.ResourcesId;
                        booking.ResourceName = resource.Name;
                    }
                }

                if (booking.TimeSlotId == 0 && !string.IsNullOrEmpty(booking.StartTime))
                    {
                        var allSlots = await _context.TimeSlots.ToListAsync();
                        var slot = allSlots.FirstOrDefault(ts => ts.startTime.ToString(@"hh\:mm") == booking.StartTime);
                        if (slot != null)
                        {
                            booking.TimeSlotId = slot.TimeSlotsId;
                            booking.StartTime = slot.startTime.ToString(@"hh\:mm");
                            booking.EndTime = slot.endTime.ToString(@"hh\:mm");
                        }
                    }

                if (string.IsNullOrEmpty(booking.Date) || booking.ResourceId == 0 || booking.TimeSlotId == 0)
                    {
                        aIChatResponse.AIResponse = "För att göra en bokning måste du ange datum, tid och resurs (t.ex. idag, imorgon, på måndag eller ett exakt datum exempelvis jag vill boka ett mötesrum imorgon kl 10)";
                        aIChatResponse.IsBookingSuggestion = false;
                        aIChatResponse.SuggestedBooking = null;
                        var aiMessage = new AIChatMessage { UserId = userId, Message = aIChatResponse.AIResponse, IsFromAI = true };
                        _context.AIChatMessages.Add(aiMessage);
                        await _context.SaveChangesAsync();
                        return Ok(aIChatResponse);
                    }

                var date = DateTime.Parse(booking.Date);
                var isFull = await _context.Bookings.AnyAsync(b => b.ResourceId == booking.ResourceId && b.Date == date && b.TimeSlotId == booking.TimeSlotId);

                if (isFull)
                {
                    aIChatResponse.AIResponse = $"Tyvärr alla tider för {booking.ResourceName} den {date:yyyy-MM-dd} är upptagna. Vill du att jag letar upp en annan passande tid?";
                    aIChatResponse.IsBookingSuggestion = false;
                }
                else
                {
                    _pendingBookings[userId] = booking;
                    aIChatResponse.AIResponse += " Vill du bekräfta bokningen?";
                }

            }

            var aiMsg = new AIChatMessage { UserId = userId, Message = aIChatResponse.AIResponse, IsFromAI = true };
            _context.AIChatMessages.Add(aiMsg);
            await _context.SaveChangesAsync();

            return Ok(aIChatResponse);
        }

        [HttpPost("Response")]
        //[Authorize]
        public async Task<IActionResult> ChatAIResponse([FromBody] UserResponse userConfirm)
        {
            // Kolla user id i jwt för att veta om användaren är inloggad.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.Trim();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token");

            // kolla om det finns en pending booking
            if (!_pendingBookings.ContainsKey(userId))
                return Unauthorized("Ingen bokning att bekräfta");

            var pending = _pendingBookings[userId];

            
            var prompt = $@"Användaren svarade '{userConfirm.UserConfirm}' på bokningsförslaget:
                {JsonSerializer.Serialize(pending)}
                Om svaret är ja, bekräfta bokningen och ge ett vänligt svar.
                Om svaret är nej, fråga om användaren vill ändra något eller avsluta.
                Returnera alltid JSON:
                {{
                    'AIResponse': 'Text till användaren',
                    'Confirmed': true/false
                }} ";


            // AI kopplas in för att ge ett svar senare
            var http = _httpClientFactory.CreateClient("openai");
            var body = new
            {
                model = "gpt-4.1",

                input = new object[]
                {
                    new {role = "system", content = prompt}
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await http.PostAsync("responses", content);
            var raw = await response.Content.ReadAsStringAsync();

           string jsonResponse = "inget svar";
            try
            {
                var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("output", out var outputElem) &&
                    outputElem.GetArrayLength() > 0 &&
                    outputElem[0].TryGetProperty("content", out var contentElem) &&
                    contentElem.GetArrayLength() > 0 &&
                    contentElem[0].TryGetProperty("text", out var textElem))
                {
                    jsonResponse = textElem.GetString() ?? "inget svar";
                }
                else
                {
                    System.Console.WriteLine("JSON-format oväntat: " + raw);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("JSON-fel: " + ex.Message);
            }
            // Tolka AI:s svar
            var aiResponse = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            bool confirmed = false;
            string aiText = "Inget svar från AI.";

            if (aiResponse.TryGetProperty("Confirmed", out var confirmedProp))
            {
                confirmed = confirmedProp.GetBoolean();
            }
            
            if(aiResponse.TryGetProperty("AIResponse", out var aiTextProp))
            {
                aiText = aiTextProp.GetString();
            }

            if (confirmed)
                {
                    // Skapa bokning
                    var booking = new Booking
                    {
                        Date = DateTime.Parse(pending.Date),
                        ResourceId = pending.ResourceId,
                        TimeSlotId = pending.TimeSlotId,
                        UserId = userId
                    };
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    _pendingBookings.Remove(userId);

                    await _hubContext.Clients.All.SendAsync("BookingCreated", booking);

                }
                else
                {
                    _pendingBookings.Remove(userId);
                }

            var aiMessage = new AIChatMessage { UserId = userId, Message = aiText, IsFromAI = true };
            _context.AIChatMessages.Add(aiMessage);
            await _context.SaveChangesAsync();

            
            return Ok(new { message = aiText, confirmed });
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> MessageHistory()
        {

            // Kolla om user id i jwt token.

            // Hämta alla chattar som är sparade för att kunna visa de. 
            return Ok();
        }

        
    }
}
