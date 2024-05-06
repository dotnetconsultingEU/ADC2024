// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

namespace dotnetconsulting.Jwt.Manager;

public static class JwtCustomClaims
{
    public const string Culture = "http://schemas.dotnetconsulting.eu/ws/2021/03/identity/claims/culture";
    public const string TransactionId = "http://schemas.dotnetconsulting.eu/ws/2021/03/identity/claims/transid";
    public const string Policy = "http://schemas.dotnetconsulting.eu/ws/2021/03/identity/claims/policy";
}