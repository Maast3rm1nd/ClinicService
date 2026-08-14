using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Contracts.Responses;
using MedicalCard = ClinicServiceContext.Entities.MedicalCardSnapshot;
using MedicalCardSnapshot = WS_ClinicService.Contracts.Responses.MedicalCardSnapshot;
using Appointment = ClinicServiceContext.Entities.AppointmentSnapshot;
using AppointmentSnapshot = WS_ClinicService.Contracts.Responses.AppointmentSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetAdminMedicalCardQuery(Guid Id) : IRequest<MedicalCardSnapshot>;

    public record AddAppointmentSlipCommand(AddAppointmentSlipRequest Request) : IRequest<AppointmentSnapshot>;

    public record UpdateAppointmentSlipCommand(Guid Id, AppointmentSnapshot AppointmentSlip) : IRequest<AppointmentSnapshot>;

    public record DeleteAppointmentSlipCommand(Guid Id) : IRequest<DeleteAppointmentSlipsResponse>;

    public class GetAdminMedicalCardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetAdminMedicalCardQuery, MedicalCardSnapshot>
    {
        public async Task<MedicalCardSnapshot> Handle(GetAdminMedicalCardQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IMedicalCardSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Медицинская карта с id {request.Id} не найдена");

            return mapper.Map<MedicalCardSnapshot>(entity);
        }
    }

    public class AddAppointmentSlipCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<AddAppointmentSlipCommand, AppointmentSnapshot>
    {
        public async Task<AppointmentSnapshot> Handle(AddAppointmentSlipCommand request, CancellationToken cancellationToken)
        {
            var scheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
            var appointmentRepository = unitOfWork.GetRepository<IAppointmentSnapshotRepository>();

            var schedule = await scheduleRepository.GetObjectsById(request.Request.ScheduleId, cancellationToken)
                ?? throw new RecordNotFoundException($"Расписание с id {request.Request.ScheduleId} не найдено");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                Doctor = schedule.Doctor,
                AppointmentDateTime = request.Request.AppointmentDateTime,
                CreatedBy = Guid.Empty,
                Status = AppointmentStatus.Pending
            };

            schedule.Appointments ??= new List<Guid>();
            schedule.Appointments.Add(appointment.Id);

            await appointmentRepository.AddObject(appointment);
            await scheduleRepository.UpdateObject(schedule);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<AppointmentSnapshot>(appointment);
        }
    }

    public class UpdateAppointmentSlipCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateAppointmentSlipCommand, AppointmentSnapshot>
    {
        public async Task<AppointmentSnapshot> Handle(UpdateAppointmentSlipCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IAppointmentSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Талон с id {request.Id} не найден");

            mapper.Map(request.AppointmentSlip, entity);

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<AppointmentSnapshot>(entity);
        }
    }

    public class DeleteAppointmentSlipCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAppointmentSlipCommand, DeleteAppointmentSlipsResponse>
    {
        public async Task<DeleteAppointmentSlipsResponse> Handle(DeleteAppointmentSlipCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IAppointmentSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return new DeleteAppointmentSlipsResponse
            {
                DeletedIds = new List<Guid> { request.Id }
            };
        }
    }
}