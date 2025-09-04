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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "http://localhost:5199/",
        ValidAudience = "http://localhost:5199/",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey != null ? Encoding.UTF8.GetBytes(secretKey) : throw new InvalidOperationException("JWT secret key is null")),
    };
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Här lägger du till statiska filer
app.UseStaticFiles();

app.UseCors("AllowFrontend"); // Använd CORS-policy

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
