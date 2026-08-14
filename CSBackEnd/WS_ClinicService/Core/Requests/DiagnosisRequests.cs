using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using Diagnosis = ClinicServiceContext.Entities.DiagnosisSnapshot;
using DiagnosisSnapshot = WS_ClinicService.Contracts.Responses.DiagnosisSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetDiagnosesQuery : IRequest<List<DiagnosisSnapshot>>;

    public record GetDiagnosisByIdQuery(Guid Id) : IRequest<DiagnosisSnapshot>;

    public record CreateDiagnosisCommand(CreateDiagnosisRequest Request) : IRequest<DiagnosisSnapshot>;

    public record UpdateDiagnosisCommand(Guid Id, DiagnosisSnapshot Diagnosis) : IRequest<DiagnosisSnapshot>;

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
                ?? throw new RecordNotFoundException($"Диагноз с id {request.Id} не найден");

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
                ?? throw new RecordNotFoundException($"Диагноз с id {request.Id} не найден");

            mapper.Map(request.Diagnosis, entity);

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