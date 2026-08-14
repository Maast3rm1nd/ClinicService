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
    [Route("admin")]
    [Authorize(Roles = "Administrator")]
    public class AdministratorsController : ControllerBase
    {
        private readonly ISender _sender;

        public AdministratorsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("medical-cards/{id:guid}")]
        public async Task<ActionResult<MedicalCardSnapshotDto>> GetMedicalCard(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetAdminMedicalCardQuery(id), cancellationToken));
        }

        [HttpPost("appointment-slips")]
        public async Task<ActionResult<AppointmentSnapshotDto>> AddAppointmentSlip([FromBody] AddAppointmentSlipRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new AddAppointmentSlipCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPut("appointment-slips/{id:guid}")]
        public async Task<ActionResult<AppointmentSnapshotDto>> EditAppointmentSlip(Guid id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateAppointmentSlipCommand(id, request), cancellationToken));
        }

        [HttpDelete("appointment-slips/{id:guid}")]
        public async Task<ActionResult<DeleteAppointmentSlipsResponse>> DeleteAppointmentSlip(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new DeleteAppointmentSlipCommand(id), cancellationToken));
        }
    }
}