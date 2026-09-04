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
    [Route("medical-cards")]
    [Authorize]
    public class MedicalCardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalCardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<MedicalCardSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<MedicalCardSnapshotDto>>> GetMedicalCards(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<MedicalCardSnapshotDto>
            {
                Data = await _mediator.Send(new GetMedicalCardsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicalCardSnapshotDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MedicalCardSnapshotDto>> CreateMedicalCard([FromBody] CreateMedicalCardRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateMedicalCardCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(MedicalCardSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MedicalCardSnapshotDto>> GetMedicalCardById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetMedicalCardByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(MedicalCardSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MedicalCardSnapshotDto>> UpdateMedicalCard(Guid id, [FromBody] UpdateMedicalCardRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateMedicalCardCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMedicalCard(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteMedicalCardCommand(id), cancellationToken);

            return NoContent();
        }
    }
}