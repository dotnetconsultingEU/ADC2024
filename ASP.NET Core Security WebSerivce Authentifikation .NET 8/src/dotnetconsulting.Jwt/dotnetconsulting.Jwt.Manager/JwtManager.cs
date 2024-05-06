// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dotnetconsulting.Jwt.Manager;

/// <summary>
/// Little helper to verify JWT for 4-eyes-principal security.
/// </summary>
public class JwtManager : IJwtManager
{
    private readonly ClaimsPrincipal _currentPrincipal;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtManager> _logger;

    public JwtManager(IHttpContextAccessor HttpContextAccessor,
                      IOptions<JwtSettings> Settings,
                      ILogger<JwtManager> Logger)
    {
        if (Settings.Value is null)
            throw new ArgumentNullException(nameof(Settings));

        _currentPrincipal = HttpContextAccessor.HttpContext.User;
        _jwtSettings = Settings.Value;
        _logger = Logger;
    }

    /// <summary>
    /// Creats the token for a user.
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    public string CreateToken(string UserId)
    {
        _logger.LogInformation("CreateToken({UserId})", UserId);

        try
        {
            // Zugriff auf User-Datenbank!

            // Authentication & data successful gathered so generate jwt token
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // Create JWT Token
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        // Well-known claim types
                        new(ClaimTypes.NameIdentifier, UserId),
                        new(ClaimTypes.GivenName, "Thorsten"),
                        new(ClaimTypes.Name, "Kansy"),
                        new(ClaimTypes.Email, "tkansy@dotnetconsulting.eu"),
                        new(ClaimTypes.Sid, "dem5Q0eY"),

                        // Rolen
                        new(ClaimTypes.Role, "RoleA"),
                        new(ClaimTypes.Role, "RoleB"),
                        new(ClaimTypes.Role, "RoleC"),

                        new(ClaimTypes.Role, "RoleDetails"),
                        new(ClaimTypes.Role, "RoleContacts"),
                        new(ClaimTypes.Role, "RoleTasks"),                        
                        new(ClaimTypes.Role, "RoleDocuments"),
                        new(ClaimTypes.Role, "RoleSecurity"),
                        new(ClaimTypes.Role, "RoleDelete"),

                        //new Claim(ClaimTypes.Role, "RoleC"),

                        new(JwtCustomClaims.Policy, "6"),

                        // Custom types
                        new(JwtCustomClaims.Culture, "de-DE")
                }),

                Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.LiveTime),
                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string result = tokenHandler.WriteToken(token);

            // Finished
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Ex: {ex}", ex);

            return null!;
        }
    }

    /// <summary>
    /// Validates the token and checks if the requested role is present.
    /// </summary>
    /// <param name="Token">The JWT token.</param>
    /// <param name="Roles">The requested roles. All must be found in the token.</param>
    /// <returns></returns>
    public (bool isValid, ClaimsPrincipal principal) ValidateRolesWithFourEye(string Token, params string[] Roles)
    {
        _logger.LogInformation("ValidateFourEye(Roles = {Roles})", string.Join(',', Roles));

        ArgumentNullException.ThrowIfNull(Token);
        if (Roles is null || Roles.Length == 0)
            throw new ArgumentNullException(nameof(Roles));

        try
        {
            // Parameter for token validation
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            // Validate token
            JwtSecurityTokenHandler handler = new();
            ClaimsPrincipal principal = handler.ValidateToken(Token, validationParameters, out var validToken);

            // Token was invalid
            if (validToken is not JwtSecurityToken)
            {
                _logger.LogWarning("Four eye token invalid.");
                return (false, null!);
            }

            // Check to see if there are two different principal to fullfill "four eyes"
            string currentPrincipalId = _currentPrincipal!.Claims!.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)!.Value;
            string principalId = principal!.Claims!.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)!.Value;
            if (string.Compare(currentPrincipalId, principalId, true) == 0)
                return (false, null!);

            // Token is valid, but are all the required roles available?
            bool rolesFound = Roles.All(role => principal.Claims
                .Any(w => w.Type == ClaimTypes.Role && string.Compare(w.Value, role, true) == 0));

            if (!rolesFound)
            {
                _logger.LogWarning("Four eye token valid, but requested role not present.");

                return (false, null!);
            }

            return (true, principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed. Excaption catched & handled.");
            return (false, null!);
        }
    }

    /// <summary>
    /// Validates the token and checks if the requested role is present. 
    /// </summary>
    /// <param name="Token">The JWT token.</param>
    /// <param name="Roles">The requested roles. All must be in the token.</param>
    /// <returns></returns>
    /// <remarks>The principal behind the token may be the same as the current principal.</remarks>
    public bool ValidateRoles(string Token, params string[] Roles)
    {
        _logger.LogInformation("Validate(Roles = {Roles})", string.Join(',', Roles));

        ArgumentNullException.ThrowIfNull(Token);
        if (Roles is null || Roles.Length == 0)
            throw new ArgumentNullException(nameof(Roles));

        try
        {
            // Parameter for token validation
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            // Validate token
            JwtSecurityTokenHandler handler = new();
            ClaimsPrincipal principal = handler.ValidateToken(Token, validationParameters, out var validToken);

            // Token was invalid
            if (validToken is not JwtSecurityToken)
            {
                _logger.LogWarning("Token invalid.");
                return false;
            }

            // Token is valid, but are all the required roles available?
            bool rolesFound = Roles.All(role => principal.Claims
                .Any(w => w.Type == ClaimTypes.Role && string.Compare(w.Value, role, true) == 0));

            return rolesFound;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed. Excaption catched & handled.");
            return false;
        }
    }
}