using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using DoctorEntity = ClinicServiceContext.Entities.Doctor;
using DoctorSnapshot = ClinicServiceBase.DTO.DoctorsDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetDoctorsQuery : IRequest<List<DoctorSnapshot>>;

    public record GetDoctorByIdQuery(Guid Id) : IRequest<DoctorSnapshot>;

    public record CreateDoctorCommand(CreateDoctorRequest Request) : IRequest<DoctorSnapshot>;

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

    public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDoctorByIdQuery, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IDoctorsRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

            return mapper.Map<DoctorSnapshot>(entity);
        }
    }

    public class CreateDoctorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateDoctorCommand, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDoctorsRepository>();

            var entity = mapper.Map<DoctorEntity>(request.Request);

            entity.Type = PersonType.Doctor;

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<DoctorSnapshot>(entity);
        }
    }

    public class UpdateDoctorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateDoctorCommand, DoctorSnapshot>
    {
        public async Task<DoctorSnapshot> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IDoctorsRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Doctor with id [{request.Id}] was not found");

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
