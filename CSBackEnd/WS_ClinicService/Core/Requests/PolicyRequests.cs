using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using Policy = ClinicServiceContext.Entities.PolicySnapshot;
using PolicySnapshot = ClinicServiceBase.DTO.PolicySnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetPoliciesQuery : IRequest<List<PolicySnapshot>>;

    public record GetPolicyByIdQuery(Guid Id) : IRequest<PolicySnapshot>;

    public record CreatePolicyCommand(CreatePolicyRequest Request) : IRequest<PolicySnapshot>;

    public record UpdatePolicyCommand(Guid Id, UpdatePolicyRequest Request) : IRequest<PolicySnapshot>;

    public record DeletePolicyCommand(Guid Id) : IRequest<Unit>;

    public class GetPoliciesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPoliciesQuery, List<PolicySnapshot>>
    {
        public async Task<List<PolicySnapshot>> Handle(GetPoliciesQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IPolicySnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<PolicySnapshot>>(items);
        }
    }

    public class GetPolicyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPolicyByIdQuery, PolicySnapshot>
    {
        public async Task<PolicySnapshot> Handle(GetPolicyByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IPolicySnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Полис с id {request.Id} не найден");

            return mapper.Map<PolicySnapshot>(entity);
        }
    }

    public class CreatePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreatePolicyCommand, PolicySnapshot>
    {
        public async Task<PolicySnapshot> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPolicySnapshotRepository>();

            var entity = mapper.Map<Policy>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PolicySnapshot>(entity);
        }
    }

    public class UpdatePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdatePolicyCommand, PolicySnapshot>
    {
        public async Task<PolicySnapshot> Handle(UpdatePolicyCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPolicySnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Полис с id {request.Id} не найден");

            if (!string.IsNullOrWhiteSpace(request.Request.MedicalPolicyNumber))
            {
                entity.MedicalPolicyNumber = request.Request.MedicalPolicyNumber;
            }

            if (request.Request.MedicalPolicyType.HasValue)
            {
                entity.MedicalPolicyType = request.Request.MedicalPolicyType.Value;
            }

            if (request.Request.InsuranceProvider.HasValue)
            {
                entity.InsuranceProvider = request.Request.InsuranceProvider.Value;
            }

            entity.Description = request.Request.Description ?? entity.Description;

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PolicySnapshot>(entity);
        }
    }

    public class DeletePolicyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeletePolicyCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePolicyCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IPolicySnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
