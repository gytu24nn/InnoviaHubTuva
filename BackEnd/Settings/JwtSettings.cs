using System;

namespace BackEnd.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string? Issuer { get; set; } = string.Empty;
    public string? Audience { get; set; } = string.Empty;
    public int ExpiresHours { get; set; } = 1;
    }
