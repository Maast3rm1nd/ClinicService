using AutoMapper;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using MediatR;
using WS_ClinicService.Contracts.Requests;
using WS_ClinicService.Core.Auth;
using Person = ClinicServiceContext.Entities.PersonSnapshot;
using PersonSnapshot = ClinicServiceBase.DTO.PersonSnapshotDto;

namespace WS_ClinicService.Core.Requests
{
    public record GetPersonsQuery : IRequest<List<PersonSnapshot>>;

    public record GetPersonByIdQuery(Guid Id) : IRequest<PersonSnapshot>;

    public record CreatePersonCommand(CreatePersonRequest Request) : IRequest<PersonSnapshot>;

    public record UpdatePersonCommand(Guid Id, UpdatePersonRequest Request) : IRequest<PersonSnapshot>;

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
                ?? throw new RecordNotFoundException($"Person with id [{request.Id}] was not found");

            return mapper.Map<PersonSnapshot>(entity);
        }
    }

    public class CreatePersonCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DatabaseAuthenticationService authenticationService) : IRequestHandler<CreatePersonCommand, PersonSnapshot>
    {
        public async Task<PersonSnapshot> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<IPersonSnapshotRepository>();

            var entity = mapper.Map<Person>(request.Request);
            entity.PasswordHash = authenticationService.HashPassword(entity, request.Request.Password);
            entity.IsCurrent = true;
            entity.IsDeleted = false;

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
                ?? throw new RecordNotFoundException($"Person with id [{request.Id}] was not found");

            if (!string.IsNullOrWhiteSpace(request.Request.FullName))
            {
                entity.FullName = request.Request.FullName;
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Login))
            {
                entity.Login = request.Request.Login;
            }

            entity.ShortName = request.Request.ShortName ?? entity.ShortName;

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
