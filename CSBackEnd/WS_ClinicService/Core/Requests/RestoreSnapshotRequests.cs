using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using MediatR;
using ClinicServiceBase.DTO;

namespace WS_ClinicService.Core.Requests
{
    public record RestoreMedicalCardCommand(Guid EntityId) : IRequest<MedicalCardSnapshotDto>;
    public record RestorePolicyCommand(Guid EntityId) : IRequest<PolicySnapshotDto>;
    public record RestoreInsuranceProviderCommand(Guid EntityId) : IRequest<InsuranceProviderSnapshotDto>;
    public record RestoreSpecialisationCommand(Guid EntityId) : IRequest<SpecialisationSnapshotDto>;
    public record RestoreDiagnosisCommand(Guid EntityId) : IRequest<DiagnosisSnapshotDto>;
    public record RestoreAppointmentCommand(Guid EntityId) : IRequest<AppointmentSnapshotDto>;
    public record RestorePersonCommand(Guid EntityId) : IRequest<PersonSnapshotDto>;

    public class RestoreMedicalCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestoreMedicalCardCommand, MedicalCardSnapshotDto>
    {
        public Task<MedicalCardSnapshotDto> Handle(RestoreMedicalCardCommand request, CancellationToken cancellationToken) => RestoreAsync<MedicalCardSnapshot, MedicalCardSnapshotDto>(request.EntityId, "medical card", cancellationToken);
        private async Task<TDto> RestoreAsync<TEntity, TDto>(Guid entityId, string name, CancellationToken cancellationToken) where TEntity : SnapshotBase
        {
            var entity = await versionService.RestoreAsync<TEntity>(entityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted {name} with entity id [{entityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<TDto>(entity);
        }
    }

    public class RestorePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestorePolicyCommand, PolicySnapshotDto>
    {
        public async Task<PolicySnapshotDto> Handle(RestorePolicyCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<PolicySnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted policy with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<PolicySnapshotDto>(entity);
        }
    }

    public class RestoreInsuranceProviderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestoreInsuranceProviderCommand, InsuranceProviderSnapshotDto>
    {
        public async Task<InsuranceProviderSnapshotDto> Handle(RestoreInsuranceProviderCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<InsuranceProviderSnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted insurance provider with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<InsuranceProviderSnapshotDto>(entity);
        }
    }

    public class RestoreSpecialisationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestoreSpecialisationCommand, SpecialisationSnapshotDto>
    {
        public async Task<SpecialisationSnapshotDto> Handle(RestoreSpecialisationCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<SpecialisationSnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted specialisation with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<SpecialisationSnapshotDto>(entity);
        }
    }

    public class RestoreDiagnosisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestoreDiagnosisCommand, DiagnosisSnapshotDto>
    {
        public async Task<DiagnosisSnapshotDto> Handle(RestoreDiagnosisCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<DiagnosisSnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted diagnosis with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<DiagnosisSnapshotDto>(entity);
        }
    }

    public class RestoreAppointmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestoreAppointmentCommand, AppointmentSnapshotDto>
    {
        public async Task<AppointmentSnapshotDto> Handle(RestoreAppointmentCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<AppointmentSnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted appointment with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<AppointmentSnapshotDto>(entity);
        }
    }

    public class RestorePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, SnapshotVersionService versionService) : IRequestHandler<RestorePersonCommand, PersonSnapshotDto>
    {
        public async Task<PersonSnapshotDto> Handle(RestorePersonCommand request, CancellationToken cancellationToken)
        {
            var entity = await versionService.RestoreAsync<PersonSnapshot>(request.EntityId, null, cancellationToken) ?? throw new RecordNotFoundException($"Deleted person with entity id [{request.EntityId}] was not found");
            await unitOfWork.CommitToDBAsync(cancellationToken);
            return mapper.Map<PersonSnapshotDto>(entity);
        }
    }
}
