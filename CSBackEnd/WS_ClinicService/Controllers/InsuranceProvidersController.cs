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
    [Route("insurance-providers")]
    [Authorize]
    public class InsuranceProvidersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InsuranceProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<InsuranceProviderSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListResponse<InsuranceProviderSnapshotDto>>> GetInsuranceProviders(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<InsuranceProviderSnapshotDto>
            {
                Data = await _mediator.Send(new GetInsuranceProvidersQuery(), cancellationToken)
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(InsuranceProviderSnapshotDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> CreateInsuranceProvider([FromBody] CreateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateInsuranceProviderCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(FilterResponse<InsuranceProviderSnapshotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FilterResponse<InsuranceProviderSnapshotDto>>> GetInsuranceProvidersByFilter(
            [FromBody] JArray filter,
            [FromQuery] int? takeCount,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc,
            CancellationToken cancellationToken)
        {
            var items = await _mediator.Send(new GetInsuranceProvidersByFilterQuery(filter, takeCount, sortBy, sortDesc), cancellationToken);

            return Ok(new FilterResponse<InsuranceProviderSnapshotDto> { Items = items });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InsuranceProviderSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> GetInsuranceProviderById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetInsuranceProviderByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(InsuranceProviderSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> UpdateInsuranceProvider(Guid id, [FromBody] UpdateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateInsuranceProviderCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInsuranceProvider(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteInsuranceProviderCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpPost("{entityId:guid}/restore")]
        [ProducesResponseType(typeof(InsuranceProviderSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> RestoreInsuranceProvider(Guid entityId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new RestoreInsuranceProviderCommand(entityId), cancellationToken));
    }
}