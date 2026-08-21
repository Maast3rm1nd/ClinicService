using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Requests;

namespace WS_ClinicService.Controllers
{
    [ApiController]
    [Route("persons")]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<PersonSnapshot>>> GetPersons(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<PersonSnapshot>
            {
                Data = await _mediator.Send(new GetPersonsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<PersonSnapshot>> CreatePerson([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreatePersonCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PersonSnapshot>> GetPersonById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetPersonByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PersonSnapshot>> UpdatePerson(Guid id, [FromBody] PersonSnapshot person, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdatePersonCommand(id, person), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePerson(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeletePersonCommand(id), cancellationToken);

            return NoContent();
        }
    }
}