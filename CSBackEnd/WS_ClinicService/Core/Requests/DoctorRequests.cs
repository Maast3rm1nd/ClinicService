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

    public record CreateDoctorCommand(CreateDoctorRequest Request) : IRequest<CreatedAccountResponse<DoctorSnapshot>>;

    public record UpdateDoctorCommand(Guid Id, UpdateDoctorRequest Request) : IRequest<DoctorSnapshot>;

    public record DeleteDoctorCommand(Guid Id) : IRequest<Unit>;

    public class GetDoctorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDoctorsQuery, List<DoctorSnapshot>>
    {
        public async Task<List<DoctorSnapshot>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IDoctorsRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<DoctorSnapshot>>(items);
        }
    }

    public class GetDoctorsByFilterQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDoctorsByFilterQuery, List<DoctorSnapshot>>
    {
        public async Task<List<DoctorSnapshot>> Handle(GetDoctorsByFilterQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IDoctorsRepository>().GetAllObjects(cancellationToken);
            var filtered = FilterQueryProcessor.Apply(items, request.Filter, request.SortBy, request.SortDesc, request.TakeCount);
            return mapper.Map<List<DoctorSnapshot>>(filtered);
        }
    }

    public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDoctorByIdQuery, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IDoctorsRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

            return mapper.Map<DoctorSnapshot>(entity);
        }
    }

    public class CreateDoctorCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DatabaseAuthenticationService authenticationService,
        PasswordSetupService passwordSetupService) :
        IRequestHandler<CreateDoctorCommand, CreatedAccountResponse<DoctorSnapshot>>
    {
        public async Task<CreatedAccountResponse<DoctorSnapshot>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
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

            await repository.AddObject(entity);
            var invitation = passwordSetupService.CreateInvitation(entity.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return new CreatedAccountResponse<DoctorSnapshot>
            {
                Account = mapper.Map<DoctorSnapshot>(entity),
                SetupToken = invitation.Token,
                SetupTokenExpiresAt = invitation.ExpiresAt
            };
        }
    }

    public class UpdateDoctorCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DatabaseAuthenticationService authenticationService) : IRequestHandler<UpdateDoctorCommand, DoctorSnapshot>
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

            return mapper.Map<DoctorSnapshot>(entity);
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
