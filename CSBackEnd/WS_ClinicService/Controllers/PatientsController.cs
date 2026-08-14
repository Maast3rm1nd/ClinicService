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
    [Route("patients")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly ISender _sender;

        public PatientsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<PatientSnapshotDto>>> GetPatients(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<PatientSnapshotDto>
            {
                Data = await _sender.Send(new GetPatientsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<PatientSnapshotDto>> CreatePatient([FromBody] CreatePatientRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreatePatientCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientSnapshotDto>> GetPatientById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetPatientByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PatientSnapshotDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdatePatientCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePatient(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeletePatientCommand(id), cancellationToken);

            return NoContent();
        }
    }
}