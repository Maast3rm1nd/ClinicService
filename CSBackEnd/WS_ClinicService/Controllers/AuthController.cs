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
    public class AuthController : ControllerBase
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        private readonly DatabaseAuthenticationService _authenticationService;

        private readonly TokenService _tokenService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly AccountSecurityService _securityService;
        private readonly SecurityAuditService _auditService;
        private readonly TotpService _totpService;

        public AuthController(
            IOptions<JwtOptions> jwtOptions,
            DatabaseAuthenticationService authenticationService,
            TokenService tokenService,
            RefreshTokenService refreshTokenService,
            AccountSecurityService securityService,
            SecurityAuditService auditService,
            TotpService totpService)
        {
            _jwtOptions = jwtOptions;
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _securityService = securityService;
            _auditService = auditService;
            _totpService = totpService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
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

            var securityState = await _securityService.GetOrCreateAsync(user.Id, cancellationToken);
            if (securityState.TwoFactorEnabled
                && (string.IsNullOrWhiteSpace(request.TwoFactorCode)
                    || securityState.TwoFactorSecret is null
                    || !_totpService.Verify(securityState.TwoFactorSecret, request.TwoFactorCode, DateTimeOffset.UtcNow)))
            {
                await _auditService.WriteAsync("login.2fa_failed", false, user.Id, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);
                return Unauthorized(new ErrorResponse { Code = StatusCodes.Status401Unauthorized, Message = "Invalid two-factor code" });
            }

            var role = GetRole(user.Type);
            var permissions = GetPermissions(role);
            var refresh = await _refreshTokenService.CreateAsync(user.Id, GetIpAddress(), Request.Headers.UserAgent, cancellationToken);
            await _auditService.WriteAsync("login.succeeded", true, user.Id, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);

            return Ok(new TokenResponse
            {
                AccessToken = _tokenService.CreateToken(user.Login, role, permissions),
                RefreshToken = refresh.Token,
                TokenType = "Bearer",
                ExpiresIn = _jwtOptions.Value.ExpiresMinutes * 60,
                RefreshTokenExpiresAt = refresh.Session.ExpiresAt
            });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var rotated = await _refreshTokenService.RotateAsync(request.RefreshToken, GetIpAddress(), Request.Headers.UserAgent, cancellationToken);
            if (rotated is null)
            {
                await _auditService.WriteAsync("token.refresh_failed", false, null, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);
                return Unauthorized(new ErrorResponse { Code = StatusCodes.Status401Unauthorized, Message = "Invalid refresh token" });
            }

            var user = await _authenticationService.GetCurrentUserAsync(rotated.Value.Session.PersonId, cancellationToken);
            if (user is null)
            {
                return Unauthorized(new ErrorResponse { Code = StatusCodes.Status401Unauthorized, Message = "User is unavailable" });
            }

            var role = GetRole(user.Type);
            return Ok(new TokenResponse
            {
                AccessToken = _tokenService.CreateToken(user.Login, role, GetPermissions(role)),
                RefreshToken = rotated.Value.Token,
                TokenType = "Bearer",
                ExpiresIn = _jwtOptions.Value.ExpiresMinutes * 60,
                RefreshTokenExpiresAt = rotated.Value.Session.ExpiresAt
            });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeAsync(request.RefreshToken, cancellationToken);
            await _auditService.WriteAsync("logout", true, null, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);
            return NoContent();
        }

        [HttpPost("2fa/setup")]
        [Authorize]
        public async Task<IActionResult> SetupTwoFactor(CancellationToken cancellationToken)
        {
            var user = await _authenticationService.GetCurrentUserAsync(User.Identity?.Name, cancellationToken);
            if (user is null)
            {
                return Unauthorized();
            }

            var state = await _securityService.GetOrCreateAsync(user.Id, cancellationToken);
            state.TwoFactorSecret = _totpService.CreateSecret();
            state.TwoFactorEnabled = false;
            await _securityService.SaveAsync(state, cancellationToken);
            return Ok(new { secret = state.TwoFactorSecret });
        }

        [HttpPost("2fa/confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmTwoFactor([FromBody] TwoFactorCodeRequest request, CancellationToken cancellationToken)
        {
            var user = await _authenticationService.GetCurrentUserAsync(User.Identity?.Name, cancellationToken);
            if (user is null)
            {
                return Unauthorized();
            }

            var state = await _securityService.GetOrCreateAsync(user.Id, cancellationToken);
            if (state.TwoFactorSecret is null || !_totpService.Verify(state.TwoFactorSecret, request.Code, DateTimeOffset.UtcNow))
            {
                return UnprocessableEntity(new ErrorResponse { Code = StatusCodes.Status422UnprocessableEntity, Message = "Invalid two-factor code" });
            }

            state.TwoFactorEnabled = true;
            state.TwoFactorConfirmedAt = DateTimeOffset.UtcNow;
            await _securityService.SaveAsync(state, cancellationToken);
            await _auditService.WriteAsync("2fa.enabled", true, user.Id, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);
            return NoContent();
        }

        [HttpPost("2fa/disable")]
        [Authorize]
        public async Task<IActionResult> DisableTwoFactor([FromBody] TwoFactorCodeRequest request, CancellationToken cancellationToken)
        {
            var user = await _authenticationService.GetCurrentUserAsync(User.Identity?.Name, cancellationToken);
            if (user is null)
            {
                return Unauthorized();
            }

            var state = await _securityService.GetOrCreateAsync(user.Id, cancellationToken);
            if (state.TwoFactorSecret is null || !_totpService.Verify(state.TwoFactorSecret, request.Code, DateTimeOffset.UtcNow))
            {
                return Unauthorized(new ErrorResponse { Code = StatusCodes.Status401Unauthorized, Message = "Invalid two-factor code" });
            }

            state.TwoFactorEnabled = false;
            state.TwoFactorSecret = null;
            state.TwoFactorConfirmedAt = null;
            await _securityService.SaveAsync(state, cancellationToken);
            await _auditService.WriteAsync("2fa.disabled", true, user.Id, GetIpAddress(), Request.Headers.UserAgent, null, cancellationToken);
            return NoContent();
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

        private static string[] GetPermissions(string role) => role switch
        {
            "Administrator" => ["users.manage", "security.audit.read", "security.sessions.revoke"],
            "Doctor" => [],
            _ => []
        };

        private string? GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}