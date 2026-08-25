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
    [Route("doctors")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<DoctorsDto>>> GetDoctors(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<DoctorsDto>
            {
                Data = await _mediator.Send(new GetDoctorsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<DoctorsDto>> CreateDoctor([FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateDoctorCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DoctorsDto>> GetDoctorById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetDoctorByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DoctorsDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateDoctorCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteDoctorCommand(id), cancellationToken);

            return NoContent();
        }
    }
}