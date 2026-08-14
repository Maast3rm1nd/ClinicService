using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using ClinicServiceContext.Entities;
using WS_ClinicService.Contracts.Requests;
using Person = ClinicServiceContext.Entities.PersonSnapshot;
using PersonSnapshot = WS_ClinicService.Contracts.Responses.PersonSnapshot;

namespace WS_ClinicService.Core.Requests
{
    public record GetPersonsQuery : IRequest<List<PersonSnapshot>>;

    public record GetPersonByIdQuery(Guid Id) : IRequest<PersonSnapshot>;

    public record CreatePersonCommand(CreatePersonRequest Request) : IRequest<PersonSnapshot>;

    public record UpdatePersonCommand(Guid Id, PersonSnapshot Person) : IRequest<PersonSnapshot>;

    public record DeletePersonCommand(Guid Id) : IRequest<Unit>;

    public class GetPersonsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPersonsQuery, List<PersonSnapshot>>
    {
        public async Task<List<PersonSnapshot>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
        {
            var items = await unitOfWork.GetRepository<IPersonSnapshotRepository>().GetAllObjects(cancellationToken);
            return mapper.Map<List<PersonSnapshot>>(items);
        }
    }

    public class GetPersonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPersonByIdQuery, PersonSnapshot>
    {
        public async Task<PersonSnapshot> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.GetRepository<IPersonSnapshotRepository>().GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Персона с id {request.Id} не найдена");

            return mapper.Map<PersonSnapshot>(entity);
        }
    }

    public class CreatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreatePersonCommand, PersonSnapshot>
    {
        public async Task<PersonSnapshot> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPersonSnapshotRepository>();

            var entity = mapper.Map<Person>(request.Request);

            await repository.AddObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PersonSnapshot>(entity);
        }
    }

    public class UpdatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdatePersonCommand, PersonSnapshot>
    {
        public async Task<PersonSnapshot> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPersonSnapshotRepository>();

            var entity = await repository.GetObjectsById(request.Id, cancellationToken)
                ?? throw new RecordNotFoundException($"Персона с id {request.Id} не найдена");

            mapper.Map(request.Person, entity);

            entity.EditDateTime = DateTimeOffset.UtcNow;

            await repository.UpdateObject(entity);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return mapper.Map<PersonSnapshot>(entity);
        }
    }

    public class DeletePersonCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeletePersonCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.GetRepository<IPersonSnapshotRepository>().SoftDeleteById(request.Id);

            await unitOfWork.CommitToDBAsync(cancellationToken);

            return Unit.Value;
        }
    }
}