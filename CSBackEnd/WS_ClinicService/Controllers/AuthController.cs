using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<AuthOptions> _authOptions;

        private readonly TokenService _tokenService;

        public AuthController(IOptions<AuthOptions> authOptions, TokenService tokenService)
        {
            _authOptions = authOptions;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _authOptions.Value.Users.FirstOrDefault(u =>
                u.Login == request.Login && u.Password == request.Password);

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
                AccessToken = _tokenService.CreateToken(user.Login, user.Role),
                TokenType = "Bearer",
                ExpiresIn = 3600
            });
        }
    }
}