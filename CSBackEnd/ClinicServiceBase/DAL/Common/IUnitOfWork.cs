using ClinicServiceBase.DAL.DBRepositories;

namespace ClinicServiceBase.DAL.Common
{
    public interface IUnitOfWork : IDisposable
    {
        void CommitToDB();

        Task CommitToDBAsync(CancellationToken cancellationToken);

        void CommitWithUpdateTime(bool updateTime);

        Task CommitWithUpdateTimeAsync(bool updateTime, CancellationToken cancellationToken);

        void Register(IClinicServiseRepository repository);

        T GetRepository<T>() where T : IClinicServiseRepository;
    }
}
