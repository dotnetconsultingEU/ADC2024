using adc2024.weather.Code;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Settings für Jwt hinzufügen
IConfigurationSection jwtConfigurationSection = builder.Configuration.GetSection("JwtSettings");
JwtSettings jwtSettings = jwtConfigurationSection.Get<JwtSettings>()!;

// Secret in ein Byte-Array umwandeln
byte[] secret = Encoding.UTF8.GetBytes(jwtSettings.Secret);

// JWT Authentifizierung hinzufügen
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secret)
    };

    // Optional
#if DEBUG
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Debugger.Break();
            context.Response.Redirect(jwtSettings.FailedUrl);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            // context.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJzdHJpbmciLCJnaXZlbl9uYW1lIjoiVGhvcnN0ZW4iLCJmYW1pbHlfbmFtZSI6IkthbnN5IiwiZW1haWwiOiJ0a2Fuc3lAZG90bmV0Y29uc3VsdGluZy5ldSIsImh0dHA6Ly9zY2hlbWFzLmRvdG5ldGNvbnN1bHRpbmcuZXUvd3MvMjAyNC8wNS9pZGVudGl0eS9jbGFpbXMvY3VsdHVyZSI6ImRlLURFIiwicm9sZSI6WyJBZG1pbiIsIlVzZXIiLCJTdXBlclVzZXIiXSwibmJmIjoxNzE1MDA2ODIzLCJleHAiOjE3MTUwOTMyMjMsImlhdCI6MTcxNTAwNjgyMywiaXNzIjoiS2Fuc3kgQUcifQ.z1hoGc9r4UQ1P_FFKuL8eOPhQ9A5d3e2oXzTSzs7Euk";
            return Task.CompletedTask;
        },
    };
#endif

});

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
