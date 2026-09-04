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
        [ProducesResponseType(typeof(ListResponse<SpecialisationSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<SpecialisationSnapshotDto>>> GetSpecialisations(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<SpecialisationSnapshotDto>
            {
                Data = await _mediator.Send(new GetSpecialisationsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(SpecialisationSnapshotDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecialisationSnapshotDto>> CreateSpecialisation([FromBody] CreateSpecialisationRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateSpecialisationCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SpecialisationSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecialisationSnapshotDto>> GetSpecialisationById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetSpecialisationByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SpecialisationSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SpecialisationSnapshotDto>> UpdateSpecialisation(Guid id, [FromBody] UpdateSpecialisationRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateSpecialisationCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSpecialisation(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteSpecialisationCommand(id), cancellationToken);

            return NoContent();
        }
    }
}