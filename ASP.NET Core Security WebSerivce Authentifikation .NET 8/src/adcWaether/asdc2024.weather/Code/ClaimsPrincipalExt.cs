using System.Security.Claims;

namespace adc2024.weather.Code
{
    public static class ClaimsPrincipalExt
    {
        public static string? UserDisplayname(this ClaimsPrincipal ClaimsPrincipal)
        {
            string? firstName = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == ClaimTypes.GivenName)?.Value;
            string? lastName = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == ClaimTypes.Name)?.Value;

            return $"{lastName}, {firstName}";
        }

        public static string? UserTransactionId(this ClaimsPrincipal ClaimsPrincipal)
        {
            string? transId = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == "TransactionId")?.Value;

            return transId;
        }
    }
}