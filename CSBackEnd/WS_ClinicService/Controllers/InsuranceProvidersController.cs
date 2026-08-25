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
        public async Task<ActionResult<ListResponse<InsuranceProviderSnapshotDto>>> GetInsuranceProviders(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<InsuranceProviderSnapshotDto>
            {
                Data = await _mediator.Send(new GetInsuranceProvidersQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> CreateInsuranceProvider([FromBody] CreateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            var created = await _mediator.Send(new CreateInsuranceProviderCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> GetInsuranceProviderById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetInsuranceProviderByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> UpdateInsuranceProvider(Guid id, [FromBody] UpdateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new UpdateInsuranceProviderCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteInsuranceProvider(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteInsuranceProviderCommand(id), cancellationToken);

            return NoContent();
        }
    }
}