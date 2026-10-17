using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using MediatR;
using Newtonsoft.Json.Linq;
using WS_ClinicService.Core.Auth;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using WS_ClinicService.Core.Filtering;
using DoctorEntity = ClinicServiceContext.Entities.Doctor;
using DoctorSnapshot = ClinicServiceBase.DTO.DoctorsDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetDoctorsQuery : IRequest<List<DoctorSnapshot>>;

    public record GetDoctorsByFilterQuery(JArray Filter, int? TakeCount, string? SortBy, bool SortDesc) : IRequest<List<DoctorSnapshot>>;

    public record GetDoctorByIdQuery(Guid Id) : IRequest<DoctorSnapshot>;

    public record CreateDoctorCommand(CreateDoctorRequest Request) : IRequest<DoctorSnapshot>;

    public record UpdateDoctorCommand(Guid Id, UpdateDoctorRequest Request) : IRequest<DoctorSnapshot>;

    public record DeleteDoctorCommand(Guid Id) : IRequest<Unit>;

    public record SetDoctorTwoFactorCommand(Guid Id, bool Enabled) : IRequest<AdminTwoFactorResponse>;

    public static class DoctorTwoFactorExtensions
    {
        public static async Task<List<DoctorSnapshot>> WithTwoFactorStatusAsync(
            this List<DoctorSnapshot> doctors,
            AccountSecurityService securityService,
            CancellationToken cancellationToken)
        {
            var enabled = await securityService.GetTwoFactorEnabledIdsAsync(doctors.Select(d => d.Id), cancellationToken);
            doctors.ForEach(d => d.TwoFactorEnabled = enabled.Contains(d.Id));
            return doctors;
        }
    }

    public class GetDoctorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, AccountSecurityService securityService) : IRequestHandler<GetDoctorsQuery, List<DoctorSnapshot>>
    {
        public async Task<List<DoctorSnapshot>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IDoctorsRepository>().GetAllObjects(cancellationToken);
            return await mapper.Map<List<DoctorSnapshot>>(items).WithTwoFactorStatusAsync(securityService, cancellationToken);
        }
    }

    public class GetDoctorsByFilterQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, AccountSecurityService securityService) : IRequestHandler<GetDoctorsByFilterQuery, List<DoctorSnapshot>>
    {
        public async Task<List<DoctorSnapshot>> Handle(GetDoctorsByFilterQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IDoctorsRepository>().GetAllObjects(cancellationToken);
            var filtered = FilterQueryProcessor.Apply(items, request.Filter, request.SortBy, request.SortDesc, request.TakeCount);
            return await mapper.Map<List<DoctorSnapshot>>(filtered).WithTwoFactorStatusAsync(securityService, cancellationToken);
        }
    }

    public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, AccountSecurityService securityService) : IRequestHandler<GetDoctorByIdQuery, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IDoctorsRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

            var dto = mapper.Map<DoctorSnapshot>(entity);
            dto.TwoFactorEnabled = await securityService.IsTwoFactorEnabledAsync(dto.Id, cancellationToken);
            return dto;
        }
    }

    public class CreateDoctorCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DatabaseAuthenticationService authenticationService,
        AccountSecurityService securityService) :
        IRequestHandler<CreateDoctorCommand, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDoctorsRepository>();

            if (await authenticationService.LoginExistsAsync(request.Request.Login, null, cancellationToken))
            {
                throw new ConflictException("Login is already in use.");
            }

            var entity = mapper.Map<DoctorEntity>(request.Request);

            entity.Type = PersonType.Doctor;
            entity.IsCurrent = true;
            entity.IsDeleted = false;
            entity.PasswordHash = authenticationService.HashPassword(entity, request.Request.Password);

            await repository.AddObject(entity);
            securityService.RequirePasswordChange(entity.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<DoctorSnapshot>(entity);
        }
    }

    public class UpdateDoctorCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DatabaseAuthenticationService authenticationService,
        AccountSecurityService securityService) : IRequestHandler<UpdateDoctorCommand, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDoctorsRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

            if (!string.IsNullOrWhiteSpace(request.Request.Login)
                && request.Request.Login != entity.Login
                && await authenticationService.LoginExistsAsync(request.Request.Login, entity.Id, cancellationToken))
            {
                throw new ConflictException("Login is already in use.");
            }

            if (!string.IsNullOrWhiteSpace(request.Request.FullName))
            {
                entity.FullName = request.Request.FullName;
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Login))
            {
                entity.Login = request.Request.Login;
            }

            entity.ShortName = request.Request.ShortName ?? entity.ShortName;

            if (request.Request.Specialisations != null)
            {
                entity.Specialisations = request.Request.Specialisations;
            }

            if (request.Request.EmployeeWorkStatus.HasValue)
            {
                entity.EmployeeWorkStatus = request.Request.EmployeeWorkStatus.Value;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            var dto = mapper.Map<DoctorSnapshot>(entity);
            dto.TwoFactorEnabled = await securityService.IsTwoFactorEnabledAsync(dto.Id, cancellationToken);
            return dto;
        }
    }

    public class SetDoctorTwoFactorCommandHandler(
        IUnitOfWork unitOfWork,
        AccountSecurityService securityService,
        TotpService totpService,
        TrustedTwoFactorDeviceService trustedDeviceService) :
        IRequestHandler<SetDoctorTwoFactorCommand, AdminTwoFactorResponse>
    {
        public async Task<AdminTwoFactorResponse> Handle(SetDoctorTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await unitOfWork.GetRepository<IDoctorsRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

            var state = await securityService.GetOrCreateAsync(doctor.Id, cancellationToken);

            if (request.Enabled)
            {
                if (state.TwoFactorEnabled)
                {
                    throw new ConflictException("Two-factor authentication is already enabled.");
                }

                state.TwoFactorSecret = totpService.CreateSecret();
                state.TwoFactorEnabled = true;
                state.TwoFactorConfirmedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                state.TwoFactorSecret = null;
                state.TwoFactorEnabled = false;
                state.TwoFactorConfirmedAt = null;
            }

            await trustedDeviceService.RevokeAllAsync(doctor.Id, cancellationToken);
            await securityService.SaveAsync(state, cancellationToken);

            return new AdminTwoFactorResponse
            {
                Enabled = state.TwoFactorEnabled,
                Secret = request.Enabled ? state.TwoFactorSecret : null
            };
        }
    }

    public class DeleteDoctorCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteDoctorCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IDoctorsRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
