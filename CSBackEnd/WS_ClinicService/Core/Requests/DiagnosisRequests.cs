using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using Diagnosis = ClinicServiceContext.Entities.DiagnosisSnapshot;
using DiagnosisSnapshot = ClinicServiceBase.DTO.DiagnosisSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetDiagnosesQuery : IRequest<List<DiagnosisSnapshot>>;

    public record GetDiagnosisByIdQuery(Guid Id) : IRequest<DiagnosisSnapshot>;

    public record CreateDiagnosisCommand(CreateDiagnosisRequest Request) : IRequest<DiagnosisSnapshot>;

    public record UpdateDiagnosisCommand(Guid Id, UpdateDiagnosisRequest Request) : IRequest<DiagnosisSnapshot>;

    public record DeleteDiagnosisCommand(Guid Id) : IRequest<Unit>;

    public class GetDiagnosesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDiagnosesQuery, List<DiagnosisSnapshot>>
    {
        public async Task<List<DiagnosisSnapshot>> Handle(GetDiagnosesQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IDiagnosisSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<DiagnosisSnapshot>>(items);
        }
    }

    public class GetDiagnosisByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDiagnosisByIdQuery, DiagnosisSnapshot>
    {
        public async Task<DiagnosisSnapshot> Handle(GetDiagnosisByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IDiagnosisSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Diagnosis with id [{request.Id}] was not found");

            return mapper.Map<DiagnosisSnapshot>(entity);
        }
    }

    public class CreateDiagnosisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateDiagnosisCommand, DiagnosisSnapshot>
    {
        public async Task<DiagnosisSnapshot> Handle(CreateDiagnosisCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDiagnosisSnapshotRepository>();

            var entity = mapper.Map<Diagnosis>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<DiagnosisSnapshot>(entity);
        }
    }

    public class UpdateDiagnosisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateDiagnosisCommand, DiagnosisSnapshot>
    {
        public async Task<DiagnosisSnapshot> Handle(UpdateDiagnosisCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDiagnosisSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Diagnosis with id [{request.Id}] was not found");

            if (!string.IsNullOrWhiteSpace(request.Request.IcdCode))
            {
                entity.IcdCode = request.Request.IcdCode;
            }

            if (request.Request.Doctor.HasValue)
            {
                entity.Doctor = request.Request.Doctor.Value;
            }

            if (request.Request.EditedDoctor.HasValue)
            {
                entity.EditedDoctor = request.Request.EditedDoctor.Value;
            }

            if (request.Request.MedicalCard.HasValue)
            {
                entity.MedicalCard = request.Request.MedicalCard.Value;
            }

            if (request.Request.Status.HasValue)
            {
                entity.Status = request.Request.Status.Value;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<DiagnosisSnapshot>(entity);
        }
    }

    public class DeleteDiagnosisCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteDiagnosisCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteDiagnosisCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IDiagnosisSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
