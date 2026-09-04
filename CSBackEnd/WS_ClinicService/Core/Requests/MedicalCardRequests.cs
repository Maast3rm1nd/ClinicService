using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using MedicalCard = ClinicServiceContext.Entities.MedicalCardSnapshot;
using MedicalCardSnapshot = ClinicServiceBase.DTO.MedicalCardSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetMedicalCardsQuery : IRequest<List<MedicalCardSnapshot>>;

    public record GetMedicalCardByIdQuery(Guid Id) : IRequest<MedicalCardSnapshot>;

    public record CreateMedicalCardCommand(CreateMedicalCardRequest Request) : IRequest<MedicalCardSnapshot>;

    public record UpdateMedicalCardCommand(Guid Id, UpdateMedicalCardRequest Request) : IRequest<MedicalCardSnapshot>;

    public record DeleteMedicalCardCommand(Guid Id) : IRequest<Unit>;

    public class GetMedicalCardsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetMedicalCardsQuery, List<MedicalCardSnapshot>>
    {
        public async Task<List<MedicalCardSnapshot>> Handle(GetMedicalCardsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IMedicalCardSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<MedicalCardSnapshot>>(items);
        }
    }

    public class GetMedicalCardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetMedicalCardByIdQuery, MedicalCardSnapshot>
    {
        public async Task<MedicalCardSnapshot> Handle(GetMedicalCardByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IMedicalCardSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Medical card with id [{request.Id}] was not found");

            return mapper.Map<MedicalCardSnapshot>(entity);
        }
    }

    public class CreateMedicalCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateMedicalCardCommand, MedicalCardSnapshot>
    {
        public async Task<MedicalCardSnapshot> Handle(CreateMedicalCardCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IMedicalCardSnapshotRepository>();

            var entity = mapper.Map<MedicalCard>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<MedicalCardSnapshot>(entity);
        }
    }

    public class UpdateMedicalCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateMedicalCardCommand, MedicalCardSnapshot>
    {
        public async Task<MedicalCardSnapshot> Handle(UpdateMedicalCardCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IMedicalCardSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Medical card with id [{request.Id}] was not found");

            if (request.Request.Patient.HasValue)
            {
                entity.Patient = request.Request.Patient.Value;
            }

            if (request.Request.Policy.HasValue)
            {
                entity.Policy = request.Request.Policy.Value;
            }

            if (request.Request.Diagnoses != null)
            {
                entity.Diagnoses = request.Request.Diagnoses;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<MedicalCardSnapshot>(entity);
        }
    }

    public class DeleteMedicalCardCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteMedicalCardCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteMedicalCardCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IMedicalCardSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
