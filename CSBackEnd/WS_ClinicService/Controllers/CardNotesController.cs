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
    [Route("card-notes")]
    [Authorize]
    public class CardNotesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CardNotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<CardNoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<CardNoteDto>>> GetCardNotes(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<CardNoteDto>
            {
                Data = await _mediator.Send(new GetCardNotesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(CardNoteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardNoteDto>> CreateCardNote([FromBody] CreateCardNoteRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateCardNoteCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(FilterResponse<CardNoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FilterResponse<CardNoteDto>>> GetCardNotesByFilter(
            [FromBody] JArray filter,
            [FromQuery] int? takeCount,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc,
            CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetCardNotesByFilterQuery(filter, takeCount, sortBy, sortDesc), cancellationToken);

            return Ok(new FilterResponse<CardNoteDto> { Items = items });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CardNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardNoteDto>> GetCardNoteById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCardNoteByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CardNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardNoteDto>> UpdateCardNote(Guid id, [FromBody] UpdateCardNoteRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateCardNoteCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCardNote(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCardNoteCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpPost("{entityId:guid}/restore")]
        [ProducesResponseType(typeof(CardNoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CardNoteDto>> RestoreCardNote(Guid entityId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new RestoreCardNoteCommand(entityId), cancellationToken));
    }
}