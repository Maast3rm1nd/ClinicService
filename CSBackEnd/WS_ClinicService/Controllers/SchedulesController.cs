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
    [Route("schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<Schedule>>> GetSchedules(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<Schedule>
            {
                Data = await _mediator.Send(new GetSchedulesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> CreateSchedule([FromBody] CreateScheduleRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateScheduleCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Schedule>> GetScheduleById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetScheduleByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Schedule>> UpdateSchedule(Guid id, [FromBody] Schedule schedule, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateScheduleCommand(id, schedule), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSchedule(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteScheduleCommand(id), cancellationToken);

            return NoContent();
        }
    }
}