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

/// <summary>
/// Little helper to verify JWT with and without 4-eyes-principal security.
/// </summary>
public interface IJwtManager
{
    /// <summary>
    /// Validates the token and checks if the requested role is present. 
    /// If posible it returns a <c>ClaimsPrincipal for further processing.</c>
    /// </summary>
    /// <param name="Token">The JWT token.</param>
    /// <param name="Roles">The requested roles. All must be in the token.</param>
    /// <returns></returns>
    /// <remarks>The principal behind the token may not be the same as the current principal.</remarks>
    (bool isValid, ClaimsPrincipal principal) ValidateRolesWithFourEye(string Token, params string[] Roles);

    /// <summary>
    /// Creats the token for a user.
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    string CreateToken(string UserId);

    /// <summary>
    /// Validates the token and checks if the requested role is present. 
    /// </summary>
    /// <param name="Token">The JWT token.</param>
    /// <param name="Roles">The requested roles. All must be in the token.</param>
    /// <returns></returns>
    /// <remarks>The principal behind the token may be the same as the current principal.</remarks>
    bool ValidateRoles(string Token, params string[] Roles);
}