using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using Patient = ClinicServiceContext.Entities.PatientSnapshot;
using PatientSnapshot = ClinicServiceBase.DTO.PatientSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetPatientsQuery : IRequest<List<PatientSnapshot>>;

    public record GetPatientByIdQuery(Guid Id) : IRequest<PatientSnapshot>;

    public record CreatePatientCommand(CreatePatientRequest Request) : IRequest<PatientSnapshot>;

    public record UpdatePatientCommand(Guid Id, UpdatePatientRequest Request) : IRequest<PatientSnapshot>;

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
                ?? throw new RecordNotFoundException($"Patient with id [{request.Id}] was not found");

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

    public class UpdatePatientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<UpdatePatientCommand, PatientSnapshot>
    {
        public async Task<PatientSnapshot> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.CreateVersionAsync<Patient>(request.Id, next =>
            {
                if (!string.IsNullOrWhiteSpace(request.Request.FullName)) next.FullName = request.Request.FullName;
                next.ShortName = request.Request.ShortName ?? next.ShortName;
                next.PassportNumber = request.Request.PassportNumber ?? next.PassportNumber;
                next.PhoneNumber = request.Request.PhoneNumber ?? next.PhoneNumber;
                next.BloodGroup = request.Request.BloodGroup ?? next.BloodGroup;
                next.Allergies = request.Request.Allergies ?? next.Allergies;
                if (request.Request.BirthDate.HasValue) next.BirthDate = request.Request.BirthDate.Value;
            }, null, cancellationToken);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PatientSnapshot>(entity);
        }
    }

    public class DeletePatientCommandHandler(IUnitOfWork unitOfWork, SnapshotVersionService versionService) : IRequestHandler<DeletePatientCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            await versionService.DeleteAsync<Patient>(request.Id, null, cancellationToken);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
