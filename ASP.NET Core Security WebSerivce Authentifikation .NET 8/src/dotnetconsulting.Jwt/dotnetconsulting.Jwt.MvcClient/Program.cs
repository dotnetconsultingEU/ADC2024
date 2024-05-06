// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

using dotnetconsulting.Jwt.Manager;
using dotnetconsulting.Jwt.MvcClient.Code;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region JWT
// Configure strongly typed settings objects for Jwt
IConfigurationSection jwtConfigurationSection = builder.Configuration.GetSection("JwtSettings");
JwtSettings jwtSettings = jwtConfigurationSection.Get<JwtSettings>()!;

byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        

        // Claims für Name und Rolle ändern
        // Entweder fest per Zuweisung
        //NameClaimType = ClaimTypes.Name,
        //RoleClaimType = ClaimTypes.Role

        // Oder per (flexibler) Logik
        //NameClaimTypeRetriever = (token, _) =>
        //{
        //    return ClaimTypes.Name;
        //},
        //RoleClaimTypeRetriever = (token, _) =>
        //{
        //    return ClaimTypes.Role;
        //}
    };

    options.Events = new JwtBearerEvents()
    {
        OnTokenValidated = (context) =>
        {
            // Custom validation
            //if (context.Principal.HasClaim(c => c.Type == "sub"))
            //{
            //    context.Fail("Custom validation failed");
            //}

            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            string? tokenKey = context.Request.Query["t"];
            if (tokenKey is null)
                context.Fail(new JwtValidationException());
            context.Token = context.Request.Cookies[$"JwtToken-{tokenKey}"];

            context.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ0a2FucyIsImdpdmVuX25hbWUiOiJUaG9yc3RlbiIsInVuaXF1ZV9uYW1lIjoiS2Fuc3kiLCJlbWFpbCI6InRrYW5zeUBkb3RuZXRjb25zdWx0aW5nLmV1IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiZGVtNVEwZVkiLCJyb2xlIjpbIlJvbGVBIiwiUm9sZUIiLCJSb2xlQyIsIlJvbGVEZXRhaWxzIiwiUm9sZUNvbnRhY3RzIiwiUm9sZVRhc2tzIiwiUm9sZURvY3VtZW50cyIsIlJvbGVTZWN1cml0eSIsIlJvbGVEZWxldGUiXSwiaHR0cDovL3NjaGVtYXMuZG90bmV0Y29uc3VsdGluZy5ldS93cy8yMDIxLzAzL2lkZW50aXR5L2NsYWltcy9wb2xpY3kiOiI2IiwiaHR0cDovL3NjaGVtYXMuZG90bmV0Y29uc3VsdGluZy5ldS93cy8yMDIxLzAzL2lkZW50aXR5L2NsYWltcy9jdWx0dXJlIjoiZGUtREUiLCJuYmYiOjE3MTQ5ODM2NzQsImV4cCI6MTcxNTU4ODQ3MSwiaWF0IjoxNzE0OTgzNjc0fQ.XwjmP-nhO_tK6_-Lw-JQGrbYN583DA7gs1bLyuanvW8";
            // Failed token
            //context.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ0a2FucyIsImdpdmVuX25hbWUiOiJKYW1lcyIsInVuaXF1ZV9uYW1lIjoiQm9uZCIsImVtYWlsIjoidGthbnN5QGRvdG5ldGNvbnN1bHRpbmcuZXUiLCJodHRwOi8vc2NoZW1hcy5kb3RuZXRjb25zdWx0aW5nLmV1L3dzLzIwMjEvMDMvaWRlbnRpdHkvY2xhaW1zL2N1bHR1cmUiOiJkZS1ERSIsImh0dHA6Ly9zY2hlbWFzLmRvdG5ldGNvbnN1bHRpbmcuZXUvd3MvMjAyMS8wMy9pZGVudGl0eS9jbGFpbXMvdHJhbnNpZCI6IjY3ODkwIiwibmJmIjoxNjE2ODcwNDgzLCJleHAiOjE2MTc0NzUyODMsImlhdCI6MTYxNjg3MDQ4M30.AvzXucMyz4vWUv21im0IMHup1DmvZSClYpjshJuEFWw";

            return Task.CompletedTask;
        }
    #if DEBUG
        ,OnAuthenticationFailed = context =>
        {
            System.Diagnostics.Debugger.Break();
            return Task.CompletedTask;
        }
    #endif
    };
});
#endregion

builder.Services.AddHttpContextAccessor();

// App erstellen und Http request Pipeline konfigurieren
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Authentification failed
app.UseStatusCodePages(context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
        response.Redirect(jwtSettings.FailedUrl);

    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Security
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();