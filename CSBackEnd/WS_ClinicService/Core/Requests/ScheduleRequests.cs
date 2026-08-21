using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceBase.DTO;
using ClinicServiceContext.Entities;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using ScheduleEntity = ClinicServiceContext.Entities.Schedule;
using ScheduleSnapshot = ClinicServiceBase.DTO.ScheduleDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetSchedulesQuery : IRequest<List<ScheduleSnapshot>>;

    public record GetScheduleByIdQuery(Guid Id) : IRequest<ScheduleSnapshot>;

    public record CreateScheduleCommand(CreateScheduleRequest Request) : IRequest<ScheduleSnapshot>;

    public record UpdateScheduleCommand(Guid Id, UpdateScheduleRequest Request) : IRequest<ScheduleSnapshot>;

    public record DeleteScheduleCommand(Guid Id) : IRequest<Unit>;

    public class GetSchedulesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetSchedulesQuery, List<ScheduleSnapshot>>
    {
        public async Task<List<ScheduleSnapshot>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IScheduleRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<ScheduleSnapshot>>(items);
        }
    }

    public class GetScheduleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetScheduleByIdQuery, ScheduleSnapshot>
    {
        public async Task<ScheduleSnapshot> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IScheduleRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Расписание с id {request.Id} не найдено");

            return mapper.Map<ScheduleSnapshot>(entity);
        }
    }

    public class CreateScheduleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateScheduleCommand, ScheduleSnapshot>
    {
        public async Task<ScheduleSnapshot> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IScheduleRepository>();

            var entity = mapper.Map<ScheduleEntity>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<ScheduleSnapshot>(entity);
        }
    }

    public class UpdateScheduleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateScheduleCommand, ScheduleSnapshot>
    {
        public async Task<ScheduleSnapshot> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IScheduleRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Расписание с id {request.Id} не найдено");

            if (request.Request.Doctor.HasValue)
            {
                entity.Doctor = request.Request.Doctor.Value;
            }

            if (request.Request.Appointments != null)
            {
                entity.Appointments = request.Request.Appointments;
            }

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<ScheduleSnapshot>(entity);
        }
    }

    public class DeleteScheduleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteScheduleCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IScheduleRepository>().DeleteObjectById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
