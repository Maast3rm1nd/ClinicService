using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using Newtonsoft.Json.Linq;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Core.Filtering;

namespace WS_ClinicService.Core.Requests
{
    public record GetCardNotesQuery : IRequest<List<CardNoteDto>>;

    public record GetCardNotesByFilterQuery(JArray Filter, int? TakeCount, string? SortBy, bool SortDesc) : IRequest<List<CardNoteDto>>;

    public record GetCardNoteByIdQuery(Guid Id) : IRequest<CardNoteDto>;

    public record CreateCardNoteCommand(CreateCardNoteRequest Request) : IRequest<CardNoteDto>;

    public record UpdateCardNoteCommand(Guid Id, UpdateCardNoteRequest Request) : IRequest<CardNoteDto>;

    public record DeleteCardNoteCommand(Guid Id) : IRequest<Unit>;

    public class GetCardNotesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCardNotesQuery, List<CardNoteDto>>
    {
        public async Task<List<CardNoteDto>> Handle(GetCardNotesQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<ICardNoteRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<CardNoteDto>>(items);
        }
    }

    public class GetCardNotesByFilterQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCardNotesByFilterQuery, List<CardNoteDto>>
    {
        public async Task<List<CardNoteDto>> Handle(GetCardNotesByFilterQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<ICardNoteRepository>().GetAllObjects(cancellationToken);
            var filtered = FilterQueryProcessor.Apply(items, request.Filter, request.SortBy, request.SortDesc, request.TakeCount);
            return mapper.Map<List<CardNoteDto>>(filtered);
        }
    }

    public class GetCardNoteByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCardNoteByIdQuery, CardNoteDto>
    {
        public async Task<CardNoteDto> Handle(GetCardNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<ICardNoteRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Card note with id [{request.Id}] was not found");

            return mapper.Map<CardNoteDto>(entity);
        }
    }

    public class CreateCardNoteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCardNoteCommand, CardNoteDto>
    {
        public async Task<CardNoteDto> Handle(CreateCardNoteCommand request, CancellationToken cancellationToken)
        {
            var noteRepository = unitOfWork.GetRepository<ICardNoteRepository>();
            var cardRepository = unitOfWork.GetRepository<IMedicalCardSnapshotRepository>();
            var diagnosisRepository = unitOfWork.GetRepository<IDiagnosisSnapshotRepository>();

            var card = await cardRepository.GetObjectsByFilter(c => c.EntityId == request.Request.MedicalCard, cancellationToken)
                ?? throw new RecordNotFoundException($"Medical card with entity id [{request.Request.MedicalCard}] was not found");

            var cardDiagnoses = (await diagnosisRepository.GetAllObjects(cancellationToken))
                .Where(d => d.MedicalCard == card.EntityId)
                .ToList();

            var diagnosisIds = new List<Guid>();

            foreach (var id in request.Request.Diagnoses ?? [])
            {
                if (cardDiagnoses.All(d => d.EntityId != id))
                {
                    throw new UnprocessableEntityException($"Diagnosis with entity id [{id}] does not belong to the medical card");
                }

                if (!diagnosisIds.Contains(id))
                {
                    diagnosisIds.Add(id);
                }
            }

            foreach (var item in request.Request.NewDiagnoses ?? [])
            {
                var diagnosis = cardDiagnoses.FirstOrDefault(d => string.Equals(d.IcdCode, item.IcdCode, StringComparison.OrdinalIgnoreCase));

                if (diagnosis is null)
                {
                    diagnosis = new DiagnosisSnapshot
                    {
                        IcdCode = item.IcdCode,
                        Name = item.Name,
                        Doctor = request.Request.Doctor,
                        MedicalCard = card.EntityId,
                        Status = item.Status ?? DiagnosisStatus.Actual
                    };

                    await diagnosisRepository.AddObject(diagnosis);
                    cardDiagnoses.Add(diagnosis);
                }

                if (!diagnosisIds.Contains(diagnosis.EntityId))
                {
                    diagnosisIds.Add(diagnosis.EntityId);
                }
            }

            // Every diagnosis of the note must be listed in the medical card of the patient.
            var cardDiagnosisIds = (card.Diagnoses ?? []).ToList();
            var missingInCard = diagnosisIds.Where(id => !cardDiagnosisIds.Contains(id)).ToList();

            if (missingInCard.Count > 0)
            {
                card.Diagnoses = cardDiagnosisIds.Concat(missingInCard).ToList();
                card.EditDateTime = DateTimeOffset.UtcNow;
                await cardRepository.UpdateObject(card);
            }

            var entity = mapper.Map<CardNote>(request.Request);
            entity.Diagnoses = diagnosisIds;

            await noteRepository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<CardNoteDto>(entity);
        }
    }

    public class UpdateCardNoteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCardNoteCommand, CardNoteDto>
    {
        public async Task<CardNoteDto> Handle(UpdateCardNoteCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ICardNoteRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Card note with id [{request.Id}] was not found");

            if (request.Request.DateNote.HasValue)
            {
                entity.DateNote = request.Request.DateNote.Value;
            }

            if (request.Request.PatientsComplaints != null)
            {
                entity.PatientsComplaints = request.Request.PatientsComplaints;
            }

            if (request.Request.Diagnoses != null)
            {
                entity.Diagnoses = request.Request.Diagnoses;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<CardNoteDto>(entity);
        }
    }

    public class DeleteCardNoteCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCardNoteCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCardNoteCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<ICardNoteRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
