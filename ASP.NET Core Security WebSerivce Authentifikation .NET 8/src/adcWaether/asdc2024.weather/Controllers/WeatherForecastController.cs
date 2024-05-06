using adc2024.weather.Code;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asdc2024.weather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WeatherForecastController(IHttpContextAccessor HttpContextAccessor,
                                           ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ClaimsPrincipal _claimsPrincipal = HttpContextAccessor.HttpContext.User;
        private readonly ILogger<WeatherForecastController> _logger = logger;

        //[Authorize(Roles = "RoleB,RoleC")]  // (RoleB oder RoleC)
        //[Authorize(Roles = "RoleA")] // und RoleA
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            ClaimsPrincipal _currentPrincipal = _claimsPrincipal;

            _ = _currentPrincipal.UserDisplayname();
            _ = _currentPrincipal.UserTransactionId();

            _ = _currentPrincipal.IsInRole("RoleA");
            _ = _currentPrincipal.IsInRole("RoleB");
            _ = _currentPrincipal.IsInRole("RoleC");



            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
