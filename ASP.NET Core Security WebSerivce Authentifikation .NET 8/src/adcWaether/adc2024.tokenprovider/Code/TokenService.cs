using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace adc2024.tokenprovider.Code;

public class TokenService(IOptions<JwtSettings> options) : ITokenService
{
    private readonly JwtSettings _options = options.Value;

    public string CreateToken(string UserId, string Password)
    {
        // Db-Abfrage

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                // Stammdaten
                new(ClaimTypes.NameIdentifier, UserId),
                new(ClaimTypes.GivenName, "Thorsten"),
                new(ClaimTypes.Surname, "Kansy"),
                new(ClaimTypes.Email, "tkansy@dotnetconsulting.eu"),
                new(JwtCustomClaims.Culture, "de-DE"),

                // Rollen
                new(ClaimTypes.Role, "Admin"),
                new(ClaimTypes.Role, "User"),
                new(ClaimTypes.Role, "SuperUser")
            }),

            Expires = DateTime.UtcNow.AddSeconds(_options.LiveTime),

            Issuer = "Kansy AG",

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                           SecurityAlgorithms.HmacSha256Signature)
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();      
        SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        string token = jwtSecurityTokenHandler.WriteToken(securityToken);

        return token;
    }
}