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
    [Route("insurance-providers")]
    [Authorize]
    public class InsuranceProvidersController : ControllerBase
    {
        private readonly ISender _sender;

        public InsuranceProvidersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<InsuranceProviderSnapshotDto>>> GetInsuranceProviders(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<InsuranceProviderSnapshotDto>
            {
                Data = await _sender.Send(new GetInsuranceProvidersQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> CreateInsuranceProvider([FromBody] CreateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateInsuranceProviderCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> GetInsuranceProviderById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetInsuranceProviderByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<InsuranceProviderSnapshotDto>> UpdateInsuranceProvider(Guid id, [FromBody] UpdateInsuranceProviderRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateInsuranceProviderCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteInsuranceProvider(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteInsuranceProviderCommand(id), cancellationToken);

            return NoContent();
        }
    }
}