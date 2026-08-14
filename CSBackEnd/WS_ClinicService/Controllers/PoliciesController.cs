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
    [Route("policies")]
    [Authorize]
    public class PoliciesController : ControllerBase
    {
        private readonly ISender _sender;

        public PoliciesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<PolicySnapshotDto>>> GetPolicies(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<PolicySnapshotDto>
            {
                Data = await _sender.Send(new GetPoliciesQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<PolicySnapshotDto>> CreatePolicy([FromBody] CreatePolicyRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreatePolicyCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PolicySnapshotDto>> GetPolicyById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetPolicyByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PolicySnapshotDto>> UpdatePolicy(Guid id, [FromBody] UpdatePolicyRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdatePolicyCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePolicy(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeletePolicyCommand(id), cancellationToken);

            return NoContent();
        }
    }
}