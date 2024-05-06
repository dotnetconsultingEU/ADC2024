using adc2024.tokenprovider.Code;
using Microsoft.AspNetCore.Mvc;

namespace adc2024.tokenprovider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenProviderController(ITokenService tokenService,
                                         ILogger<TokenProviderController> logger) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<TokenProviderController> _logger = logger;

        [HttpGet(nameof(AuthenticateUser))]
        public ActionResult<string> AuthenticateUser(string UserId, string Password)
        {
            _logger.LogInformation("AuthenticateUser called with UserId: {UserId}", UserId);

            string? token = _tokenService.CreateToken(UserId, Password);

            return token is null ? Forbid() : (ActionResult<string>)token;
        }
    }
}