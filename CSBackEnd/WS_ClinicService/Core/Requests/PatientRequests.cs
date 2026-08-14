using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using Patient = ClinicServiceContext.Entities.PatientSnapshot;
using PatientSnapshot = WS_ClinicService.Contracts.Responses.PatientSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetPatientsQuery : IRequest<List<PatientSnapshot>>;

    public record GetPatientByIdQuery(Guid Id) : IRequest<PatientSnapshot>;

    public record CreatePatientCommand(CreatePatientRequest Request) : IRequest<PatientSnapshot>;

    public record UpdatePatientCommand(Guid Id, PatientSnapshot Patient) : IRequest<PatientSnapshot>;

    public record DeletePatientCommand(Guid Id) : IRequest<Unit>;

    public class GetPatientsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPatientsQuery, List<PatientSnapshot>>
    {
        public async Task<List<PatientSnapshot>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IPatientSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<PatientSnapshot>>(items);
        }
    }

    public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPatientByIdQuery, PatientSnapshot>
    {
        public async Task<PatientSnapshot> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IPatientSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Пациент с id {request.Id} не найден");

            return mapper.Map<PatientSnapshot>(entity);
        }
    }

    public class CreatePatientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreatePatientCommand, PatientSnapshot>
    {
        public async Task<PatientSnapshot> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPatientSnapshotRepository>();

            var entity = mapper.Map<Patient>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PatientSnapshot>(entity);
        }
    }

    public class UpdatePatientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdatePatientCommand, PatientSnapshot>
    {
        public async Task<PatientSnapshot> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPatientSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Пациент с id {request.Id} не найден");

            mapper.Map(request.Patient, entity);

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PatientSnapshot>(entity);
        }
    }

    public class DeletePatientCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeletePatientCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPatientSnapshotRepository>();

            await repository.SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}