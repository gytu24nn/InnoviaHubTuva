using System;

namespace BackEnd.Models.AI;

// Resultat från SmartTips-endpoint
public class SmartTipsResponse
{
    // AI-generat tips som skrivs ut i kalender för att hjälpa användaren.
    public string tip { get; set; } = string.Empty;
    // Här samlas all statistik som AI behöver för att genera ett svar ex antal bokningar på dag.
    public object? stats { get; set; }
}
