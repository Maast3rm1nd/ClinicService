using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Requests;
using ClinicServiceBase.DTO;

namespace WS_ClinicService.Controllers
{
    [ApiController]
    [Route("specialisations")]
    [Authorize]
    public class SpecialisationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SpecialisationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<SpecialisationSnapshotDto>>> GetSpecialisations(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<SpecialisationSnapshotDto>
            {
                Data = await _mediator.Send(new GetSpecialisationsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<SpecialisationSnapshotDto>> CreateSpecialisation([FromBody] CreateSpecialisationRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateSpecialisationCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SpecialisationSnapshotDto>> GetSpecialisationById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetSpecialisationByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SpecialisationSnapshotDto>> UpdateSpecialisation(Guid id, [FromBody] UpdateSpecialisationRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateSpecialisationCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSpecialisation(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteSpecialisationCommand(id), cancellationToken);

            return NoContent();
        }
    }
}