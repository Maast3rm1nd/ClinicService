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
    [Route("schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly ISender _sender;

        public SchedulesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<ScheduleDto>>> GetSchedules(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<ScheduleDto>
            {
                Data = await _sender.Send(new GetSchedulesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule([FromBody] CreateScheduleRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateScheduleCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ScheduleDto>> GetScheduleById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetScheduleByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ScheduleDto>> UpdateSchedule(Guid id, [FromBody] UpdateScheduleRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateScheduleCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSchedule(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteScheduleCommand(id), cancellationToken);

            return NoContent();
        }
    }
}