using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using InsuranceProvider = ClinicServiceContext.Entities.InsuranceProviderSnapshot;
using InsuranceProviderSnapshot = WS_ClinicService.Contracts.Responses.InsuranceProviderSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetInsuranceProvidersQuery : IRequest<List<InsuranceProviderSnapshot>>;

    public record GetInsuranceProviderByIdQuery(Guid Id) : IRequest<InsuranceProviderSnapshot>;

    public record CreateInsuranceProviderCommand(CreateInsuranceProviderRequest Request) : IRequest<InsuranceProviderSnapshot>;

    public record UpdateInsuranceProviderCommand(Guid Id, InsuranceProviderSnapshot Provider) : IRequest<InsuranceProviderSnapshot>;

    public record DeleteInsuranceProviderCommand(Guid Id) : IRequest<Unit>;

    public class GetInsuranceProvidersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetInsuranceProvidersQuery, List<InsuranceProviderSnapshot>>
    {
        public async Task<List<InsuranceProviderSnapshot>> Handle(GetInsuranceProvidersQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<InsuranceProviderSnapshot>>(items);
        }
    }

    public class GetInsuranceProviderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetInsuranceProviderByIdQuery, InsuranceProviderSnapshot>
    {
        public async Task<InsuranceProviderSnapshot> Handle(GetInsuranceProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Страховая компания с id {request.Id} не найдена");

            return mapper.Map<InsuranceProviderSnapshot>(entity);
        }
    }

    public class CreateInsuranceProviderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateInsuranceProviderCommand, InsuranceProviderSnapshot>
    {
        public async Task<InsuranceProviderSnapshot> Handle(CreateInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>();

            var entity = mapper.Map<InsuranceProvider>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<InsuranceProviderSnapshot>(entity);
        }
    }

    public class UpdateInsuranceProviderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateInsuranceProviderCommand, InsuranceProviderSnapshot>
    {
        public async Task<InsuranceProviderSnapshot> Handle(UpdateInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Страховая компания с id {request.Id} не найдена");

            mapper.Map(request.Provider, entity);

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<InsuranceProviderSnapshot>(entity);
        }
    }

    public class DeleteInsuranceProviderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteInsuranceProviderCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}