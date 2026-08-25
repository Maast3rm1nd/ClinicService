using ClinicServiceContext.Entities;
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
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<PatientSnapshot>>> GetPatients(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<PatientSnapshot>
            {
                Data = await _mediator.Send(new GetPatientsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<PatientSnapshot>> CreatePatient([FromBody] CreatePatientRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreatePatientCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientSnapshot>> GetPatientById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetPatientByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PatientSnapshot>> UpdatePatient(Guid id, [FromBody] PatientSnapshot patient, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdatePatientCommand(id, patient), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePatient(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeletePatientCommand(id), cancellationToken);

            return NoContent();
        }
    }
}