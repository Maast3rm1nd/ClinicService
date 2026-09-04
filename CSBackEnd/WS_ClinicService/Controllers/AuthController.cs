using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using ClinicServiceContext.Enums;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Auth;

namespace WS_ClinicService.Controllers
{
    [ApiController]
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        private readonly DatabaseAuthenticationService _authenticationService;

        private readonly TokenService _tokenService;

        public AuthController(
            IOptions<JwtOptions> jwtOptions,
            DatabaseAuthenticationService authenticationService,
            TokenService tokenService)
        {
            _jwtOptions = jwtOptions;
            _authenticationService = authenticationService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authenticationService.AuthenticateAsync(
                request.Login,
                request.Password,
                cancellationToken);

            if (user == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    Code = StatusCodes.Status401Unauthorized,
                    Message = "Invalid credentials"
                });
            }

            return Ok(new TokenResponse
            {
                AccessToken = _tokenService.CreateToken(user.Login, GetRole(user.Type)),
                TokenType = "Bearer",
                ExpiresIn = _jwtOptions.Value.ExpiresMinutes * 60
            });
        }

        private static string GetRole(PersonType type)
        {
            return type switch
            {
                PersonType.Administrator => "Administrator",
                PersonType.Doctor => "Doctor",
                _ => throw new Exception($"Unknown person type: {type}"),
            };
        }
    }
}