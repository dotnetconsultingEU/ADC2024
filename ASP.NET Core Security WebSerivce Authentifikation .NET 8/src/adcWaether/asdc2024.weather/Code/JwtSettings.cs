#nullable disable
namespace adc2024.weather.Code;

public class JwtSettings
{
    public string Secret { get; set; }
    public string FailedUrl { get; set; } = "https://www.google.de";
}