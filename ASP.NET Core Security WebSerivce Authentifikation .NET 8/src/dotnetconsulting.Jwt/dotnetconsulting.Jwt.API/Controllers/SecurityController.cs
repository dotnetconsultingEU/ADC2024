// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

#pragma warning disable CA1416 // Validate platform compatibility

using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using dotnetconsulting.Jwt.Manager;

namespace dotnetconsulting.Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController(ILogger<SecurityController> logger,
                              IJwtManager JwtManager) : ControllerBase
    {
        private readonly IJwtManager _jwtManager = JwtManager;

        /// <summary>
        /// Gets the details for a given user or, if userName is <c>null</c>,
        /// the currently authenticated user.
        /// </summary>
        /// <param name="userId">The userName or <c>null</c>.</param>
        [HttpGet(nameof(AuthenticateUser))]
        public ActionResult<string> AuthenticateUser(string? userId)
        {
            logger.LogInformation("AuthenticateUser({userId})", userId);

            if (userId is null)
            {
                WindowsIdentity? user = HttpContext.User.Identity as WindowsIdentity;
                userId = GetUniqueName(user!);
            }
            else
            {
                userId = "tkans";
            }

            string? token = _jwtManager.CreateToken(userId);

            return token is null ? Forbid() : (ActionResult<string>)token;
        }

        #region Misc
        private static string GetUniqueName(WindowsIdentity user)
        {
            // Return part after backslash, if any exists

            return user.Name[(user.Name.IndexOf('\\') + 1)..];
#pragma warning restore CA1416 // Validate platform compatibility
        }
        #endregion
    }
}