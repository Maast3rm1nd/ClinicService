using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Requests;

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
        public async Task<ActionResult<ListResponse<DiagnosisSnapshot>>> GetDiagnoses(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<DiagnosisSnapshot>
            {
                Data = await _mediator.Send(new GetDiagnosesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<DiagnosisSnapshot>> CreateDiagnosis([FromBody] CreateDiagnosisRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateDiagnosisCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DiagnosisSnapshot>> GetDiagnosisById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetDiagnosisByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DiagnosisSnapshot>> UpdateDiagnosis(Guid id, [FromBody] DiagnosisSnapshot diagnosis, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateDiagnosisCommand(id, diagnosis), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDiagnosis(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteDiagnosisCommand(id), cancellationToken);

            return NoContent();
        }
    }
}