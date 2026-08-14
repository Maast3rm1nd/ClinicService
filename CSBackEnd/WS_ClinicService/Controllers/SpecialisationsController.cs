using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Requests;

namespace WS_ClinicService.Controllers
{
    [ApiController]
    [Route("specialisations")]
    [Authorize]
    public class SpecialisationsController : ControllerBase
    {
        private readonly ISender _sender;

        public SpecialisationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<SpecialisationSnapshot>>> GetSpecialisations(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<SpecialisationSnapshot>
            {
                Data = await _sender.Send(new GetSpecialisationsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<SpecialisationSnapshot>> CreateSpecialisation([FromBody] CreateSpecialisationRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateSpecialisationCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SpecialisationSnapshot>> GetSpecialisationById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetSpecialisationByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SpecialisationSnapshot>> UpdateSpecialisation(Guid id, [FromBody] SpecialisationSnapshot specialisation, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateSpecialisationCommand(id, specialisation), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSpecialisation(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteSpecialisationCommand(id), cancellationToken);

            return NoContent();
        }
    }
}