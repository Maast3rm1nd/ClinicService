using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using MedicalCard = ClinicServiceContext.Entities.MedicalCardSnapshot;
using MedicalCardSnapshot = WS_ClinicService.Contracts.Responses.MedicalCardSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetMedicalCardsQuery : IRequest<List<MedicalCardSnapshot>>;

    public record GetMedicalCardByIdQuery(Guid Id) : IRequest<MedicalCardSnapshot>;

    public record CreateMedicalCardCommand(CreateMedicalCardRequest Request) : IRequest<MedicalCardSnapshot>;

    public record UpdateMedicalCardCommand(Guid Id, MedicalCardSnapshot MedicalCard) : IRequest<MedicalCardSnapshot>;

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
                ?? throw new RecordNotFoundException($"Медицинская карта с id {request.Id} не найдена");

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
                ?? throw new RecordNotFoundException($"Медицинская карта с id {request.Id} не найдена");

            mapper.Map(request.MedicalCard, entity);

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