using ClinicServiceBase.DTO;
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
        private readonly ISender _sender;

        public DiagnosesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<DiagnosisSnapshotDto>>> GetDiagnoses(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<DiagnosisSnapshotDto>
            {
                Data = await _sender.Send(new GetDiagnosesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<DiagnosisSnapshotDto>> CreateDiagnosis([FromBody] CreateDiagnosisRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateDiagnosisCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DiagnosisSnapshotDto>> GetDiagnosisById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetDiagnosisByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DiagnosisSnapshotDto>> UpdateDiagnosis(Guid id, [FromBody] UpdateDiagnosisRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateDiagnosisCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDiagnosis(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteDiagnosisCommand(id), cancellationToken);

            return NoContent();
        }
    }
}