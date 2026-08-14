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
    [Route("appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public AppointmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<AppointmentSnapshotDto>>> GetAppointments(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<AppointmentSnapshotDto>
            {
                Data = await _sender.Send(new GetAppointmentsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentSnapshotDto>> CreateAppointment([FromBody] CreateAppointmentRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateAppointmentCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AppointmentSnapshotDto>> GetAppointmentById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetAppointmentByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AppointmentSnapshotDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateAppointmentCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> CancelAppointment(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new CancelAppointmentCommand(id), cancellationToken);

            return NoContent();
        }
    }
}