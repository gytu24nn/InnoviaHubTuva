using System;

namespace BackEnd.Models.AI;

public class AIChatResponse
{
    // Svaret som AI skickar tillbaka på frågan
    public string AIResponse { get; set; } = string.Empty;
    // Denna blir true om AI förslog en bokning och väntar då på bekräftelse för att göra bokningen.
    public bool IsBookingSuggestion { get; set; }
    // koppling till pendingBooking så om AI hittade en ledig tid föreslås den här.
    public PendingBooking? SuggestedBooking { get; set; }
}
