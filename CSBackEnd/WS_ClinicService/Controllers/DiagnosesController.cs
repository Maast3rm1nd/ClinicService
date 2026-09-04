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
    [Route("diagnoses")]
    [Authorize]
    public class DiagnosesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiagnosesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<DiagnosisSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<DiagnosisSnapshotDto>>> GetDiagnoses(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<DiagnosisSnapshotDto>
            {
                Data = await _mediator.Send(new GetDiagnosesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiagnosisSnapshotDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiagnosisSnapshotDto>> CreateDiagnosis([FromBody] CreateDiagnosisRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateDiagnosisCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DiagnosisSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiagnosisSnapshotDto>> GetDiagnosisById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetDiagnosisByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DiagnosisSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiagnosisSnapshotDto>> UpdateDiagnosis(Guid id, [FromBody] UpdateDiagnosisRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateDiagnosisCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDiagnosis(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteDiagnosisCommand(id), cancellationToken);

            return NoContent();
        }
    }
}