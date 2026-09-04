using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using InsuranceProvider = ClinicServiceContext.Entities.InsuranceProviderSnapshot;
using InsuranceProviderSnapshot = ClinicServiceBase.DTO.InsuranceProviderSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetInsuranceProvidersQuery : IRequest<List<InsuranceProviderSnapshot>>;

    public record GetInsuranceProviderByIdQuery(Guid Id) : IRequest<InsuranceProviderSnapshot>;

    public record CreateInsuranceProviderCommand(CreateInsuranceProviderRequest Request) : IRequest<InsuranceProviderSnapshot>;

    public record UpdateInsuranceProviderCommand(Guid Id, UpdateInsuranceProviderRequest Request) : IRequest<InsuranceProviderSnapshot>;

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
            var ip = await unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Insurance provider with id [{request.Id}] was not found");

            return mapper.Map<InsuranceProviderSnapshot>(ip);
        }
    }

    public class CreateInsuranceProviderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateInsuranceProviderCommand, InsuranceProviderSnapshot>
    {
        public async Task<InsuranceProviderSnapshot> Handle(CreateInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>();

            var ip = mapper.Map<InsuranceProvider>(request.Request);

            await repository.AddObject(ip);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<InsuranceProviderSnapshot>(ip);
        }
    }

    public class UpdateInsuranceProviderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateInsuranceProviderCommand, InsuranceProviderSnapshot>
    {
        public async Task<InsuranceProviderSnapshot> Handle(UpdateInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IInsuranceProviderSnapshotRepository>();

            var ip = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Insurance provider with id [{request.Id}] was not found");

            if (!string.IsNullOrWhiteSpace(request.Request.Name))
            {
                ip.Name = request.Request.Name;
            }

            if (!string.IsNullOrWhiteSpace(request.Request.LicenseNumber))
            {
                ip.LicenseNumber = request.Request.LicenseNumber;
            }

            ip.PhoneNumber = request.Request.PhoneNumber ?? ip.PhoneNumber;

            ip.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(ip);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<InsuranceProviderSnapshot>(ip);
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
