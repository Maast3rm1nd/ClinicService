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
    [Route("medical-cards")]
    [Authorize]
    public class MedicalCardsController : ControllerBase
    {
        private readonly ISender _sender;

        public MedicalCardsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<MedicalCardSnapshotDto>>> GetMedicalCards(CancellationToken cancellationToken)
        {
            return Ok(new ListResponse<MedicalCardSnapshotDto>
            {
                Data = await _sender.Send(new GetMedicalCardsQuery(), cancellationToken)
            });
        }

        [HttpPost]
        public async Task<ActionResult<MedicalCardSnapshotDto>> CreateMedicalCard([FromBody] CreateMedicalCardRequest request, CancellationToken cancellationToken)
        {
            var created = await _sender.Send(new CreateMedicalCardCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedicalCardSnapshotDto>> GetMedicalCardById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new GetMedicalCardByIdQuery(id), cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MedicalCardSnapshotDto>> UpdateMedicalCard(Guid id, [FromBody] UpdateMedicalCardRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _sender.Send(new UpdateMedicalCardCommand(id, request), cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMedicalCard(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteMedicalCardCommand(id), cancellationToken);

            return NoContent();
        }
    }
}