using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        [ProducesResponseType(typeof(ListResponse<DoctorsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<DoctorsDto>>> GetDoctors(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<DoctorsDto>
            {
                Data = await _mediator.Send(new GetDoctorsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(DoctorsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorsDto>> CreateDoctor([FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateDoctorCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(FilterResponse<DoctorsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FilterResponse<DoctorsDto>>> GetDoctorsByFilter(
            [FromBody] JArray filter,
            [FromQuery] int? takeCount,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc,
            CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetDoctorsByFilterQuery(filter, takeCount, sortBy, sortDesc), cancellationToken);

            return Ok(new FilterResponse<DoctorsDto> { Items = items });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DoctorsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorsDto>> GetDoctorById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetDoctorByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DoctorsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorsDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateDoctorCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDoctor(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteDoctorCommand(id), cancellationToken);

            return NoContent();
        }
    }
}