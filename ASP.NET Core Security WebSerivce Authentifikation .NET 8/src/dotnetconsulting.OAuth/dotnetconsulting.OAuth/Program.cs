// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschr�nkungen verwendet oder ver�ndert werden.
// Jedoch wird keine Garantie �bernommen, dass eine Funktionsf�higkeit mit aktuellen und 
// zuk�nftigen API-Versionen besteht. Der Autor �bernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgef�hrt wird.
// F�r Anregungen und Fragen stehe ich jedoch gerne zur Verf�gung.

// Thorsten Kansy, www.dotnetconsulting.eu

using dotnetconsulting.OAuth.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-5.0&tabs=visual-studio
builder.Services.AddAuthentication()
        //.AddMicrosoftAccount(options =>
        //{
        //    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        //    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

        //    options.SaveTokens = true;
        //})
        .AddLinkedIn(options =>
        {            
            // https://developer.linkedin.com/
            options.ClientId = builder.Configuration["Authentication:LinkedIn:ClientId"];
            options.ClientSecret = builder.Configuration["Authentication:LinkedIn:ClientSecret"];

            // options.Scope.Add("r_liteprofile");
            // options.Scope.Add("r_emailaddress");

            options.SaveTokens = true; 
        })
        //.AddGoogle(options =>
        //{
        //    IConfigurationSection googleAuthNSection =
        //        builder.Configuration.GetSection("Authentication:Google");

        //    options.ClientId = googleAuthNSection["ClientId"];
        //    options.ClientSecret = googleAuthNSection["ClientSecret"];

        //    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        //    options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");

        //    options.SaveTokens = true;
        //})
        ;

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    // requires using Microsoft.AspNetCore.Http;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddControllersWithViews();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapRazorPages();
//});

app.Run();