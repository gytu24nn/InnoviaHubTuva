using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackEnd.Models;
using BackEnd.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using DotNetEnv;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Innovia HUB", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("InnoviaHubDbConnection");

builder.Services.AddDbContext<InnoviaHubDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Konfigurera Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<InnoviaHubDbContext>()
    .AddDefaultTokenProviders();

// JWT configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
    throw new Exception("JWT secret is missing in configuration");

// Konfigurera JWT‑autentisering
var secretKey = jwtSettings.Secret;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    var issuer = string.IsNullOrWhiteSpace(jwtSettings?.Issuer) ? "http://localhost:5099" : jwtSettings!.Issuer!;
    var audience = string.IsNullOrWhiteSpace(jwtSettings?.Audience) ? "http://localhost:5099" : jwtSettings!.Audience!;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey != null ? Encoding.UTF8.GetBytes(secretKey) : throw new InvalidOperationException("JWT secret key is null")),
        ClockSkew = TimeSpan.FromMinutes(1)
    };


    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var hasAuthHeader = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]);
            if (!hasAuthHeader)
            {
                var cookieToken = context.Request.Cookies["auth"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("http://127.0.0.1:5173","http://localhost:5173", "https://innvoiafrontend-bfhzfqenfdh5b7d5.swedencentral-01.azurewebsites.net")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

builder.Services.AddSignalR();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Här lägger du till statiska filer
app.UseStaticFiles();

app.UseCors("AllowFrontend"); // Använd CORS-policy

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<BookingHub>("/BookingHub");
app.MapHub<NotificationHub>("/NotificationHub");

app.Run();
