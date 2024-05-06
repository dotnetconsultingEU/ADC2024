// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

using dotnetconsulting.Jwt.Manager;

var builder = WebApplication.CreateBuilder(args);

// Konfiguration
//builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//    .AddEnvironmentVariables()
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
//    .AddUserSecrets<Program>(optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region JWT
// Configure strongly typed settings objects
IConfigurationSection appSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(appSettingsSection);
builder.Services.AddTransient<IJwtManager, JwtManager>();
#endregion

builder.Services.AddHttpContextAccessor();

// App erstellen und Http request Pipeline konfigurieren
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();