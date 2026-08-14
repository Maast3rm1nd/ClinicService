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
    [Route("persons")]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        private readonly ISender _sender;

        public PersonsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<PersonSnapshotDto>>> GetPersons(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<PersonSnapshotDto>
            {
                Data = await _sender.Send(new GetPersonsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<PersonSnapshotDto>> CreatePerson([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreatePersonCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PersonSnapshotDto>> GetPersonById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetPersonByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PersonSnapshotDto>> UpdatePerson(Guid id, [FromBody] UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdatePersonCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePerson(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeletePersonCommand(id), cancellationToken);

            return NoContent();
        }
    }
}