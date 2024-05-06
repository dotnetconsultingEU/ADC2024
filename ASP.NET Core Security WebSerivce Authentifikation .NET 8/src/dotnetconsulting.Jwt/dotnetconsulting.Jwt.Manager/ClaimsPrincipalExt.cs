// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

using System.Security.Claims;

namespace dotnetconsulting.Jwt.Manager;

public static class ClaimsPrincipalExt
{
    public static string? UserId(this ClaimsPrincipal ClaimsPrincipal)
    {
        return ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type! == ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? UserDisplayname(this ClaimsPrincipal ClaimsPrincipal)
    {
        string? firstName = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == ClaimTypes.GivenName)?.Value;
        string? lastName = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == ClaimTypes.Name)?.Value;

        return $"{lastName}, {firstName}";
    }

    public static string UserCulture(this ClaimsPrincipal ClaimsPrincipal, string FallbackCulture = null!)
    {
        string? culture = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == JwtCustomClaims.Culture)?.Value;

        return culture ?? FallbackCulture;
    }

    public static string? UserTransactionId(this ClaimsPrincipal ClaimsPrincipal)
    {
        string? transId = ClaimsPrincipal?.Claims?.FirstOrDefault(w => w.Type == JwtCustomClaims.TransactionId)?.Value;

        return transId;
    }
}