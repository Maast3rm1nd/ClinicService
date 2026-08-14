using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using Specialisation = ClinicServiceContext.Entities.SpecialisationSnapshot;
using SpecialisationSnapshot = ClinicServiceBase.DTO.SpecialisationSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetSpecialisationsQuery : IRequest<List<SpecialisationSnapshot>>;

    public record GetSpecialisationByIdQuery(Guid Id) : IRequest<SpecialisationSnapshot>;

    public record CreateSpecialisationCommand(CreateSpecialisationRequest Request) : IRequest<SpecialisationSnapshot>;

    public record UpdateSpecialisationCommand(Guid Id, UpdateSpecialisationRequest Request) : IRequest<SpecialisationSnapshot>;

    public record DeleteSpecialisationCommand(Guid Id) : IRequest<Unit>;

    public class GetSpecialisationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetSpecialisationsQuery, List<SpecialisationSnapshot>>
    {
        public async Task<List<SpecialisationSnapshot>> Handle(GetSpecialisationsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<ISpecialisationSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<SpecialisationSnapshot>>(items);
        }
    }

    public class GetSpecialisationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetSpecialisationByIdQuery, SpecialisationSnapshot>
    {
        public async Task<SpecialisationSnapshot> Handle(GetSpecialisationByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<ISpecialisationSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Специализация с id {request.Id} не найдена");

            return mapper.Map<SpecialisationSnapshot>(entity);
        }
    }

    public class CreateSpecialisationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateSpecialisationCommand, SpecialisationSnapshot>
    {
        public async Task<SpecialisationSnapshot> Handle(CreateSpecialisationCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ISpecialisationSnapshotRepository>();

            var entity = mapper.Map<Specialisation>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<SpecialisationSnapshot>(entity);
        }
    }

    public class UpdateSpecialisationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateSpecialisationCommand, SpecialisationSnapshot>
    {
        public async Task<SpecialisationSnapshot> Handle(UpdateSpecialisationCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ISpecialisationSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Специализация с id {request.Id} не найдена");

            if (!string.IsNullOrWhiteSpace(request.Request.Name))
            {
                entity.Name = request.Request.Name;
            }

            if (request.Request.Doctors != null)
            {
                entity.Doctors = request.Request.Doctors;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<SpecialisationSnapshot>(entity);
        }
    }

    public class DeleteSpecialisationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSpecialisationCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteSpecialisationCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<ISpecialisationSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
