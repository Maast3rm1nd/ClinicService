using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using Appointment = ClinicServiceContext.Entities.AppointmentSnapshot;
using AppointmentSnapshot = ClinicServiceBase.DTO.AppointmentSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetAppointmentsQuery : IRequest<List<AppointmentSnapshot>>;

    public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentSnapshot>;

    public record CreateAppointmentCommand(CreateAppointmentRequest Request) : IRequest<AppointmentSnapshot>;

    public record UpdateAppointmentCommand(Guid Id, UpdateAppointmentRequest Request) : IRequest<AppointmentSnapshot>;

    public record CancelAppointmentCommand(Guid Id) : IRequest<Unit>;

    public class GetAppointmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAppointmentsQuery, List<AppointmentSnapshot>>
    {
        public async Task<List<AppointmentSnapshot>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IAppointmentSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<AppointmentSnapshot>>(items);
        }
    }

    public class GetAppointmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAppointmentByIdQuery, AppointmentSnapshot>
    {
        public async Task<AppointmentSnapshot> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IAppointmentSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Приём с id {request.Id} не найден");

            return mapper.Map<AppointmentSnapshot>(entity);
        }
    }

    public class CreateAppointmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateAppointmentCommand, AppointmentSnapshot>
    {
        public async Task<AppointmentSnapshot> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IAppointmentSnapshotRepository>();

            var entity = mapper.Map<Appointment>(request.Request);

            await EnsureNoConflict(repository, entity, cancellationToken);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<AppointmentSnapshot>(entity);
        }

        private static async Task EnsureNoConflict(
            IAppointmentSnapshotRepository repository,
            Appointment entity,
            CancellationToken cancellationToken)
        {
            var conflict = await repository.GetObjectsByFilter(
                a => a.Id != entity.Id
                     && a.Doctor == entity.Doctor
                     && a.AppointmentDateTime == entity.AppointmentDateTime
                     && !a.IsDeleted,
                cancellationToken);

            if (conflict != null)
            {
                throw new ConflictException(
                    $"Врач занят: на {entity.AppointmentDateTime:O} уже существует приём (id {conflict.Id})");
            }
        }
    }

    public class UpdateAppointmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateAppointmentCommand, AppointmentSnapshot>
    {
        public async Task<AppointmentSnapshot> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IAppointmentSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Приём с id {request.Id} не найден");

            if (request.Request.AppointmentDateTime.HasValue
                && request.Request.AppointmentDateTime.Value != entity.AppointmentDateTime)
            {
                await EnsureNoConflict(repository, entity, request.Request.AppointmentDateTime.Value, cancellationToken);

                entity.AppointmentDateTime = request.Request.AppointmentDateTime.Value;
            }

            entity.EditedBy = request.Request.EditedBy;

            if (request.Request.Status.HasValue)
            {
                entity.Status = request.Request.Status.Value;
            }

            if (request.Request.PreliminaryReason != null)
            {
                entity.PremilinaryReason = request.Request.PreliminaryReason;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<AppointmentSnapshot>(entity);
        }

        private static async Task EnsureNoConflict(
            IAppointmentSnapshotRepository repository,
            Appointment entity,
            DateTimeOffset appointmentDateTime,
            CancellationToken cancellationToken)
        {
            var conflict = await repository.GetObjectsByFilter(
                a => a.Id != entity.Id
                     && a.Doctor == entity.Doctor
                     && a.AppointmentDateTime == appointmentDateTime
                     && !a.IsDeleted,
                cancellationToken);

            if (conflict != null)
            {
                throw new ConflictException(
                    $"Врач занят: на {appointmentDateTime:O} уже существует приём (id {conflict.Id})");
            }
        }
    }

    public class CancelAppointmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelAppointmentCommand, Unit>
    {
        public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IAppointmentSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
