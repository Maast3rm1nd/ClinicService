using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using ScheduleEntity = ClinicServiceContext.Entities.Schedule;
using ScheduleSnapshot = WS_ClinicService.Contracts.Responses.Schedule;

namespace WS_ClinicService.Core.Requests
{
    public record GetSchedulesQuery : IRequest<List<ScheduleSnapshot>>;

    public record GetScheduleByIdQuery(Guid Id) : IRequest<ScheduleSnapshot>;

    public record CreateScheduleCommand(CreateScheduleRequest Request) : IRequest<ScheduleSnapshot>;

    public record UpdateScheduleCommand(Guid Id, ScheduleSnapshot Schedule) : IRequest<ScheduleSnapshot>;

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

            mapper.Map(request.Schedule, entity);

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
            await unitOfWork.GetRepository<IScheduleRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}