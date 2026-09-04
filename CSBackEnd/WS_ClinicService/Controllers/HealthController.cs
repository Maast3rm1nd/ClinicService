using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Responses;

namespace WS_ClinicService.Controllers
{
    [ApiController]
    [Route("health")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            return Ok(new HealthResponse
            {
                Status = "ok",
                Timestamp = DateTimeOffset.UtcNow
            });
        }
    }
}