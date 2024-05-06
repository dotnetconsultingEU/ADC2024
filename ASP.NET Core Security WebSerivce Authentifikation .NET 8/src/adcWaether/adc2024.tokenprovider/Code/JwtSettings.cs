#nullable disable
namespace adc2024.tokenprovider.Code;

public class JwtSettings
{
    public string Secret { get; set; }
    public int LiveTime { get; set; } = 86_400; // Tag
}